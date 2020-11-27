namespace Hans

open ICSharpCode.Decompiler.CSharp.TypeSystem
open ICSharpCode.Decompiler.CSharp.Resolver
open ICSharpCode.Decompiler.Semantics

module Disassembler =
    open System.Reflection.Metadata
    open Aardvark.Base
    open System
    open System.Reflection
    open ICSharpCode.Decompiler
    open ICSharpCode.Decompiler.Metadata
    open ICSharpCode.Decompiler.TypeSystem
    open ICSharpCode.Decompiler.CSharp.Syntax
    open Microsoft.FSharp.Quotations

    type Type with  
        member t.One =
            if t = typeof<int8> then 1y :> obj
            elif t = typeof<int16> then 1s :> obj
            elif t = typeof<int32> then 1 :> obj
            elif t = typeof<int64> then 1L :> obj
            elif t = typeof<uint8> then 1uy :> obj
            elif t = typeof<uint16> then 1us :> obj
            elif t = typeof<uint32> then 1u :> obj
            elif t = typeof<uint64> then 1UL :> obj
            elif t = typeof<float32> then 1.0f :> obj
            elif t = typeof<float> then 1.0 :> obj
            elif t = typeof<decimal> then 1.0m :> obj
            else
                let one = t.GetProperty("One")
                if isNull one || one.PropertyType <> t then failwithf "cannot create range for type: %A" t
                else one.GetValue(null)


    type Bla() =
        static member Run(a : Aardvark.Base.V2i) =
            if a.X > 10 then
                for i in 0 .. 10 do
                    System.Console.WriteLine("asdasd {0}", a, i, a, a, a)

    //type Resolver() =
    //    interface IAssemblyResolver with
    //        member this.IsGacAssembly(reference: IAssemblyReference): bool = 
    //            false

    //        member this.Resolve(reference: IAssemblyReference): PEFile = 
    //            try
    //                let ass = Assembly.Load reference.FullName
    //            with _ ->
    //                null

    //        member x.ResolveModule(parent : PEFile, name : string) =
    //            null

    let operators =
        Type.GetType "Microsoft.FSharp.Core.Operators, FSharp.Core"


    type Ex =
        | Block of list<Ex>
        | Seq of list<Ex>
        //| Expression of Ex

        | Type of Type
        | Member of Ex * name : string
        | Invoke of Ex * args : list<Ex>
        | Call of option<Ex> * MethodInfo * list<Ex>
        | Value of obj * Type
        | Lambda of vars : list<Var> * body : Ex
        | EVar of Var
        | Declare of Var * Ex
        | NewArray of Type * list<Ex>
        | Item of Ex * list<Ex>
        | Assign of Ex * Ex
        | IfThenElse of Ex * Ex * Ex
        | For of list<Ex> * Ex * list<Ex> * Ex
        | Null

    type Visitor(file : IDecompilerTypeSystem) =
        static let allStatic = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static
        static let allInstance = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance

        let rec toType (test : IType) : Type = 
            match test with
            | :? ArrayType as t ->
                let res = toType t.ElementType
                if t.Dimensions > 1 then res.MakeArrayType(t.Dimensions)
                else res.MakeArrayType()
            | _ ->
                let def = test.GetDefinition()
           
                let assName = def.FullTypeName.ReflectionName + "," + def.ParentModule.FullAssemblyName
                Type.GetType assName

        let getType (e : AbstractAnnotatable) =
            let ann = e.Annotation<ResolveResult>()
            if isNull ann then
                printfn "no type: %A" e
                null
            else
                toType ann.Type

        let mutable vars : Map<string, Var> = Map.empty

        let using (v : list<Var>) (action : unit -> 'a) =
            let o = vars
            try
                for v in v do
                    vars <- Map.add v.Name v vars
                action()
            finally 
                vars <- o



        interface IAstVisitor<Ex> with
            member this.VisitSyntaxTree(syntaxTree: SyntaxTree) =
                syntaxTree.FirstChild.AcceptVisitor this

            member this.VisitUsingDeclaration(usingDeclaration: UsingDeclaration) =
                usingDeclaration.NextSibling.AcceptVisitor this

            member this.VisitBlockStatement(blockStatement: BlockStatement) =
                using [] (fun () ->
                    blockStatement.Children
                    |> Seq.toList 
                    |> List.map (fun v -> v.AcceptVisitor this)
                    |> List.collect (function Seq s -> s | e -> [e])
                    |> Ex.Block
                )

            member this.VisitExpressionStatement(expressionStatement: ExpressionStatement) =
                expressionStatement.Expression.AcceptVisitor this
                //|> Ex.Expression

            member this.VisitMemberReferenceExpression(memberReferenceExpression: MemberReferenceExpression) =  
                let target = memberReferenceExpression.Target.AcceptVisitor this
                Ex.Member(target, memberReferenceExpression.MemberName)

            member this.VisitInvocationExpression(invocationExpression: InvocationExpression) =
                let meth = invocationExpression.Annotation<CSharpInvocationResolveResult>()
                // TODO: use meth
                printfn "%A" meth.Member
                let argTypes = invocationExpression.Arguments |> Seq.map (fun a -> getType a) |> Seq.toArray
                let args = invocationExpression.Arguments |> Seq.map (fun a -> a.AcceptVisitor this) |> Seq.toList
                let target = invocationExpression.Target.AcceptVisitor this
                match target with
                | Member(Type t, name) ->
                    let meth =
                        t.GetMethod(name, allStatic, Type.DefaultBinder, CallingConventions.Any, argTypes, null)
                    Ex.Call(None, meth, args)
                | Member(ex, name) ->
                    let t = getType invocationExpression.Target
                    let meth =
                        t.GetMethod(name, allInstance, Type.DefaultBinder, CallingConventions.Any, argTypes, null)
                    Ex.Call(Some ex, meth, args)
                | _ ->
                    failwithf "bad invocation"

            member this.VisitMethodDeclaration(methodDeclaration: MethodDeclaration) =
                let args = methodDeclaration.Parameters |> Seq.map (fun p -> Var(p.Name, getType p)) |> Seq.toList
                let body = 
                    using args (fun () ->
                        methodDeclaration.Body.AcceptVisitor this
                    )
                Ex.Lambda(args, body)

            member this.VisitIdentifierExpression(identifierExpression: IdentifierExpression) =
                match Map.tryFind identifierExpression.Identifier vars with
                | Some v -> Ex.EVar v
                | _ -> failwithf "unknown identifier: %A" identifierExpression


            member this.VisitPrimitiveExpression(primitiveExpression: PrimitiveExpression) =
                let res = primitiveExpression.Annotation<ConstantResolveResult>()
                
                Ex.Value(primitiveExpression.Value, toType res.Type)
                
            member this.VisitTypeReferenceExpression(typeReferenceExpression: TypeReferenceExpression) =
                let t = getType typeReferenceExpression
                Ex.Type t

            member this.VisitArrayCreateExpression(arrayCreateExpression: ArrayCreateExpression) = 
                let t = getType arrayCreateExpression.Type
                let lens = arrayCreateExpression.Arguments |> Seq.map (fun a -> a.AcceptVisitor this) |> Seq.toList
                Ex.NewArray(t.GetElementType(), lens)

            member this.VisitAssignmentExpression(assignmentExpression: AssignmentExpression) = 
                let l = assignmentExpression.Left.AcceptVisitor this
                let r = assignmentExpression.Right.AcceptVisitor this
                Ex.Assign(l, r)

            member this.VisitIndexerExpression(indexerExpression: IndexerExpression) =
                let self = indexerExpression.Target.AcceptVisitor this
                let indices = indexerExpression.Arguments |> Seq.map (fun i -> i.AcceptVisitor this) |> Seq.toList
                Ex.Item(self, indices)

            member this.VisitVariableDeclarationStatement(variableDeclarationStatement: VariableDeclarationStatement) =
                let vs = variableDeclarationStatement.Variables |> Seq.map (fun v -> Var(v.Name, getType v), v.Initializer.AcceptVisitor this) |> Seq.toList
                let decls = vs |> List.map Ex.Declare
                for v,_ in vs do
                    vars <- Map.add v.Name v vars
                Seq decls

            member this.VisitIfElseStatement(ifElseStatement: IfElseStatement) =
                let c = ifElseStatement.Condition.AcceptVisitor this
                let i = ifElseStatement.TrueStatement.AcceptVisitor this
                let e = ifElseStatement.FalseStatement.AcceptVisitor this

                Ex.IfThenElse(c, i, e)
                
            member this.VisitBinaryOperatorExpression(binaryOperatorExpression: BinaryOperatorExpression) =
                let lt = getType binaryOperatorExpression.Left
                let rt = getType binaryOperatorExpression.Right
                let vt = getType binaryOperatorExpression
                let l = binaryOperatorExpression.Left.AcceptVisitor this
                let r = binaryOperatorExpression.Right.AcceptVisitor this
                
                match binaryOperatorExpression.Operator with
                | BinaryOperatorType.Add ->
                    let op = operators.GetMethod "op_Addition"
                    let op = op.MakeGenericMethod [| lt; rt; vt |]
                    Ex.Call(None, op, [l;r])
                    
                | BinaryOperatorType.GreaterThan ->
                    let op = operators.GetMethod "op_GreaterThan"
                    let op = op.MakeGenericMethod [| lt |]
                    Ex.Call(None, op, [l;r])
                    
                | BinaryOperatorType.LessThanOrEqual ->
                    let op = operators.GetMethod "op_LessThanOrEqual"
                    let op = op.MakeGenericMethod [| lt |]
                    Ex.Call(None, op, [l;r])
                | BinaryOperatorType.LessThan ->
                    let op = operators.GetMethod "op_LessThan"
                    let op = op.MakeGenericMethod [| lt |]
                    Ex.Call(None, op, [l;r])
                | _ ->
                    failwith "asdjaldksakld"
            member this.VisitNullNode(nullNode: AstNode) =
                Ex.Null
                
            member this.VisitForStatement(forStatement: ForStatement) =
                using [] (fun () ->
                    let init = forStatement.Initializers |> Seq.map (fun s -> s.AcceptVisitor this) |> Seq.toList
                    let cond = forStatement.Condition.AcceptVisitor this
                    let step = forStatement.Iterators |> Seq.map (fun s -> s.AcceptVisitor this) |> Seq.toList
                    let body = forStatement.EmbeddedStatement.AcceptVisitor this
                    Ex.For(init, cond, step, body)
                )
                
            member this.VisitUnaryOperatorExpression(unaryOperatorExpression: UnaryOperatorExpression) =
                let t = getType unaryOperatorExpression.Expression
                let self = unaryOperatorExpression.Expression.AcceptVisitor this
                match unaryOperatorExpression.Operator with
                | UnaryOperatorType.PostIncrement ->
                    
                    let op = operators.GetMethod "op_Addition"
                    Ex.Assign(self, Ex.Call(None, op.MakeGenericMethod [| t; t; t |], [self; Ex.Value(t.One, t)]))
                | _ ->
                    raise (System.NotImplementedException())

            member x.VisitAccessor(accessor : Accessor) = 
                raise (System.NotImplementedException())
            member this.VisitAnonymousMethodExpression(anonymousMethodExpression: AnonymousMethodExpression) = 
                raise (System.NotImplementedException())
            member this.VisitAnonymousTypeCreateExpression(anonymousTypeCreateExpression: AnonymousTypeCreateExpression)= 
                raise (System.NotImplementedException())
            member this.VisitArrayInitializerExpression(arrayInitializerExpression: ArrayInitializerExpression) = 
                raise (System.NotImplementedException())
            member this.VisitArraySpecifier(arraySpecifier: ArraySpecifier) = 
                raise (System.NotImplementedException())
            member this.VisitAsExpression(asExpression: AsExpression) = 
                raise (System.NotImplementedException())
            member this.VisitAttribute(attribute: Attribute) = 
                raise (System.NotImplementedException())
            member this.VisitAttributeSection(attributeSection: AttributeSection) = 
                raise (System.NotImplementedException())
            member this.VisitBaseReferenceExpression(baseReferenceExpression: BaseReferenceExpression) = 
                raise (System.NotImplementedException())
            member this.VisitBreakStatement(breakStatement: BreakStatement) =
                raise (System.NotImplementedException())
            member this.VisitCSharpTokenNode(cSharpTokenNode: CSharpTokenNode) =
                raise (System.NotImplementedException())
            member this.VisitCaseLabel(caseLabel: CaseLabel) =
                raise (System.NotImplementedException())
            member this.VisitCastExpression(castExpression: CastExpression) =
                raise (System.NotImplementedException())
            member this.VisitCatchClause(catchClause: CatchClause) =
                raise (System.NotImplementedException())
            member this.VisitCheckedExpression(checkedExpression: CheckedExpression) =
                raise (System.NotImplementedException())
            member this.VisitCheckedStatement(checkedStatement: CheckedStatement) =
                raise (System.NotImplementedException())
            member this.VisitComment(comment: Comment) =
                raise (System.NotImplementedException())
            member this.VisitComposedType(composedType: ComposedType) =
                raise (System.NotImplementedException())
            member this.VisitConditionalExpression(conditionalExpression: ConditionalExpression) =
                raise (System.NotImplementedException())
            member this.VisitConstraint(``constraint``: Constraint) =
                raise (System.NotImplementedException())
            member this.VisitConstructorDeclaration(constructorDeclaration: ConstructorDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitConstructorInitializer(constructorInitializer: ConstructorInitializer) =
                raise (System.NotImplementedException())
            member this.VisitContinueStatement(continueStatement: ContinueStatement) =
                raise (System.NotImplementedException())
            member this.VisitCustomEventDeclaration(customEventDeclaration: CustomEventDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitDeclarationExpression(declarationExpression: DeclarationExpression) =
                raise (System.NotImplementedException())
            member this.VisitDefaultValueExpression(defaultValueExpression: DefaultValueExpression) =
                raise (System.NotImplementedException())
            member this.VisitDelegateDeclaration(delegateDeclaration: DelegateDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitDestructorDeclaration(destructorDeclaration: DestructorDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitDirectionExpression(directionExpression: DirectionExpression) =
                raise (System.NotImplementedException())
            member this.VisitDoWhileStatement(doWhileStatement: DoWhileStatement) =
                raise (System.NotImplementedException())
            member this.VisitDocumentationReference(documentationReference: DocumentationReference) =
                raise (System.NotImplementedException())
            member this.VisitEmptyStatement(emptyStatement: EmptyStatement) =
                raise (System.NotImplementedException())
            member this.VisitEnumMemberDeclaration(enumMemberDeclaration: EnumMemberDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitErrorNode(errorNode: AstNode) =
                raise (System.NotImplementedException())
            member this.VisitEventDeclaration(eventDeclaration: EventDeclaration) =
                raise (System.NotImplementedException())
            
            member this.VisitExternAliasDeclaration(externAliasDeclaration: ExternAliasDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitFieldDeclaration(fieldDeclaration: FieldDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitFixedFieldDeclaration(fixedFieldDeclaration: FixedFieldDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitFixedStatement(fixedStatement: FixedStatement) =
                raise (System.NotImplementedException())
            member this.VisitFixedVariableInitializer(fixedVariableInitializer: FixedVariableInitializer) =
                raise (System.NotImplementedException())
            member this.VisitForeachStatement(foreachStatement: ForeachStatement) =
                raise (System.NotImplementedException())
            member this.VisitFunctionPointerType(functionPointerType: FunctionPointerAstType) =
                raise (System.NotImplementedException())
            member this.VisitGotoCaseStatement(gotoCaseStatement: GotoCaseStatement) =
                raise (System.NotImplementedException())
            member this.VisitGotoDefaultStatement(gotoDefaultStatement: GotoDefaultStatement) =
                raise (System.NotImplementedException())
            member this.VisitGotoStatement(gotoStatement: GotoStatement) =
                raise (System.NotImplementedException())
            member this.VisitIdentifier(identifier: Identifier) =
                raise (System.NotImplementedException())
            
            member this.VisitIndexerDeclaration(indexerDeclaration: IndexerDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitInterpolatedStringExpression(interpolatedStringExpression: InterpolatedStringExpression) =
                raise (System.NotImplementedException())
            member this.VisitInterpolatedStringText(interpolatedStringText: InterpolatedStringText) =
                raise (System.NotImplementedException())
            member this.VisitInterpolation(interpolation: Interpolation) =
                raise (System.NotImplementedException())
            member this.VisitIsExpression(isExpression: IsExpression) =
                raise (System.NotImplementedException())
            member this.VisitLabelStatement(labelStatement: LabelStatement) =
                raise (System.NotImplementedException())
            member this.VisitLambdaExpression(lambdaExpression: LambdaExpression) =
                raise (System.NotImplementedException())
            member this.VisitLocalFunctionDeclarationStatement(localFunctionDeclarationStatement: LocalFunctionDeclarationStatement) =
                raise (System.NotImplementedException())
            member this.VisitLockStatement(lockStatement: LockStatement) =
                raise (System.NotImplementedException())
            member this.VisitMemberType(memberType: MemberType) =
                raise (System.NotImplementedException())
            member this.VisitNamedArgumentExpression(namedArgumentExpression: NamedArgumentExpression) =
                raise (System.NotImplementedException())
            member this.VisitNamedExpression(namedExpression: NamedExpression) =
                raise (System.NotImplementedException())
            member this.VisitNamespaceDeclaration(namespaceDeclaration: NamespaceDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitNullReferenceExpression(nullReferenceExpression: NullReferenceExpression) =
                raise (System.NotImplementedException())
            member this.VisitObjectCreateExpression(objectCreateExpression: ObjectCreateExpression) =
                raise (System.NotImplementedException())
            member this.VisitOperatorDeclaration(operatorDeclaration: OperatorDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitOutVarDeclarationExpression(outVarDeclarationExpression: OutVarDeclarationExpression) =
                raise (System.NotImplementedException())
            member this.VisitParameterDeclaration(parameterDeclaration: ParameterDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitParenthesizedExpression(parenthesizedExpression: ParenthesizedExpression) =
                raise (System.NotImplementedException())
            member this.VisitParenthesizedVariableDesignation(parenthesizedVariableDesignation: ParenthesizedVariableDesignation) =
                raise (System.NotImplementedException())
            member this.VisitPatternPlaceholder(placeholder: AstNode, pattern: PatternMatching.Pattern) =
                raise (System.NotImplementedException())
            member this.VisitPointerReferenceExpression(pointerReferenceExpression: PointerReferenceExpression) =
                raise (System.NotImplementedException())
            member this.VisitPreProcessorDirective(preProcessorDirective: PreProcessorDirective) =
                raise (System.NotImplementedException())
            member this.VisitPrimitiveType(primitiveType: PrimitiveType) =
                raise (System.NotImplementedException())
            member this.VisitPropertyDeclaration(propertyDeclaration: PropertyDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitQueryContinuationClause(queryContinuationClause: QueryContinuationClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryExpression(queryExpression: QueryExpression) =
                raise (System.NotImplementedException())
            member this.VisitQueryFromClause(queryFromClause: QueryFromClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryGroupClause(queryGroupClause: QueryGroupClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryJoinClause(queryJoinClause: QueryJoinClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryLetClause(queryLetClause: QueryLetClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryOrderClause(queryOrderClause: QueryOrderClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryOrdering(queryOrdering: QueryOrdering) =
                raise (System.NotImplementedException())
            member this.VisitQuerySelectClause(querySelectClause: QuerySelectClause) =
                raise (System.NotImplementedException())
            member this.VisitQueryWhereClause(queryWhereClause: QueryWhereClause) =
                raise (System.NotImplementedException())
            member this.VisitReturnStatement(returnStatement: ReturnStatement) =
                raise (System.NotImplementedException())
            member this.VisitSimpleType(simpleType: SimpleType) =
                raise (System.NotImplementedException())
            member this.VisitSingleVariableDesignation(singleVariableDesignation: SingleVariableDesignation) =
                raise (System.NotImplementedException())
            member this.VisitSizeOfExpression(sizeOfExpression: SizeOfExpression) =
                raise (System.NotImplementedException())
            member this.VisitStackAllocExpression(stackAllocExpression: StackAllocExpression) =
                raise (System.NotImplementedException())
            member this.VisitSwitchExpression(switchExpression: SwitchExpression) =
                raise (System.NotImplementedException())
            member this.VisitSwitchExpressionSection(switchExpressionSection: SwitchExpressionSection) =
                raise (System.NotImplementedException())
            member this.VisitSwitchSection(switchSection: SwitchSection) =
                raise (System.NotImplementedException())
            member this.VisitSwitchStatement(switchStatement: SwitchStatement) =
                raise (System.NotImplementedException())
            member this.VisitThisReferenceExpression(thisReferenceExpression: ThisReferenceExpression) =
                raise (System.NotImplementedException())
            member this.VisitThrowExpression(throwExpression: ThrowExpression) =
                raise (System.NotImplementedException())
            member this.VisitThrowStatement(throwStatement: ThrowStatement) =
                raise (System.NotImplementedException())
            member this.VisitTryCatchStatement(tryCatchStatement: TryCatchStatement) =
                raise (System.NotImplementedException())
            member this.VisitTupleExpression(tupleExpression: TupleExpression) =
                raise (System.NotImplementedException())
            member this.VisitTupleType(tupleType: TupleAstType) =
                raise (System.NotImplementedException())
            member this.VisitTupleTypeElement(tupleTypeElement: TupleTypeElement) =
                raise (System.NotImplementedException())
            member this.VisitTypeDeclaration(typeDeclaration: TypeDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitTypeOfExpression(typeOfExpression: TypeOfExpression) =
                raise (System.NotImplementedException())
            member this.VisitTypeParameterDeclaration(typeParameterDeclaration: TypeParameterDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitUncheckedExpression(uncheckedExpression: UncheckedExpression) =
                raise (System.NotImplementedException())
            member this.VisitUncheckedStatement(uncheckedStatement: UncheckedStatement) =
                raise (System.NotImplementedException())
            member this.VisitUndocumentedExpression(undocumentedExpression: UndocumentedExpression) =
                raise (System.NotImplementedException())
            member this.VisitUnsafeStatement(unsafeStatement: UnsafeStatement) =
                raise (System.NotImplementedException())
            member this.VisitUsingAliasDeclaration(usingAliasDeclaration: UsingAliasDeclaration) =
                raise (System.NotImplementedException())
            member this.VisitUsingStatement(usingStatement: UsingStatement) =
                raise (System.NotImplementedException())
            
            member this.VisitVariableInitializer(variableInitializer: VariableInitializer) =
                raise (System.NotImplementedException())
            member this.VisitWhileStatement(whileStatement: WhileStatement) =
                raise (System.NotImplementedException())
            member this.VisitYieldBreakStatement(yieldBreakStatement: YieldBreakStatement) =
                raise (System.NotImplementedException())
            member this.VisitYieldReturnStatement(yieldReturnStatement: YieldReturnStatement) =
                raise (System.NotImplementedException())




    let run() =
        let mi = typeof<Bla>.GetMethod "Run"

        let t = mi.DeclaringType
        let ass = t.Assembly
        let settings = DecompilerSettings()
        settings.AggressiveInlining <- true
        settings.AnonymousTypes <- false
        settings.UsingDeclarations <- false
        settings.UsingStatement <- false
        settings.RemoveDeadCode <- true
        settings.ArrayInitializers <- false
        settings.ObjectOrCollectionInitializers <- false
        settings.ExtensionMethods <- false
        settings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls <- true
        settings.DictionaryInitializers <- false
        settings.ObjectOrCollectionInitializers <- false
        settings.IntroduceIncrementAndDecrement <- true

        //let file = new PEFile(ass.Location)
        //let resolver = new UniversalAssemblyResolver(ass.Location, true, "netcoreapp3.1")
        let decompiler = CSharp.CSharpDecompiler(ass.Location, settings)
        let pars = mi.GetParameters()
        let typ = decompiler.TypeSystem.FindType(t)
        let meth = typ.GetMethods(fun m -> m.Name = mi.Name && m.Parameters.Count = pars.Length) |> Seq.tryHead // TODO: proper resolution
        match meth with
        | Some meth -> 
            let def = decompiler.Decompile(meth.MetadataToken)
            printfn "%A" def

            let sys = decompiler.TypeSystem

            let test = 

                def.AcceptVisitor (Visitor(sys))

            printfn "%A" test
            ()
        | None ->
            printfn "bad"
        //let h = Metadata.EntityHandle.op_Explicit def
        //let h = def :> Metadata.EntityHandle
        //decompiler.Decompile(def)


        ()