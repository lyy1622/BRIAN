namespace tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open MathNet.Numerics.LinearAlgebra
open AST
open Parser
open Evaluator

(**
* Test suite for parser.
*)
[<TestClass>]
type TestParser () =

   [<TestMethod>]
    member this.TestMethod0 () =
        let input = "A activates b
                            b activates *c"
        let expected = [Activation (Protein "A", Gene "b");
                                        Activation (Gene "b", Phenotype "*c")]
        let result = parse input
        match result with
        | Some ast ->
            Assert.AreEqual(expected, ast)
        | None -> Assert.IsTrue(false)

    [<TestMethod>]
    member this.TestMethod1 () =
       let input = "A activates #unknown
                            b inhibits *c"
       let expected = [Activation (Protein "A", Other "#unknown");
                                       Inhibition (Gene "b", Phenotype "*c")]
       let result = parse input
       match result with
       | Some ast ->
           Assert.AreEqual(expected, ast)
       | None -> Assert.IsTrue(false)

    [<TestMethod>]
    member this.TestMethod2 () =
       let input = "A hates b
                            b inhibits *c"
       let result = parse input
       // since there is no 'hates' relationship in our language,
       // test should fail if an AST is produced
       match result with
       | Some ast -> Assert.IsTrue(false)
       | None -> Assert.IsTrue(true)

(**
* Test suite for evaluator.
*)
[<TestClass>]
type TestEvaluator () =

    // test to see if 'prettyprint' function is working correctly
    [<TestMethod>]
    member this.TestMethod3 () = 
        let input = [Activation (Protein "A", Gene "b");
                                        Activation (Gene "b", Phenotype "*c")]
        let result = prettyprint input
        let expected = "A activates b\nb activates *c\n"
        Assert.AreEqual(result, expected)

    // test to see if 'getUniqueVariableList' function is working correctly
    [<TestMethod>]
    member this.TestMethod4 () = 
        let input = [Activation (Protein "A", Gene "b");
                                    Activation (Gene "b", Phenotype "*c")]
        let result = getUniqueVariableList input
        let expected = [Protein "A"; Gene "b"; Phenotype "*c"]
        Assert.AreEqual(result, expected)
    
    // test to see if 'initializeMatrix' function is working correctly
    [<TestMethod>]
    member this.TestMethod5 () = 
        let input = [Activation (Protein "A", Gene "b");
                                    Activation (Gene "b", Phenotype "*c")]
        let result = initializeMatrix input
        let x = array2D [[|double(0); 1; 0|]; [|0; 0; 1|]; [|0; 0; 0|]]
        let expected = Matrix<double>.Build.DenseOfArray(x)
        Assert.AreEqual(result, expected)

    // test to see if 'getMatrixDerivatives' function is working correctly
    [<TestMethod>]
    member this.TestMethod6 () = 
        let x = array2D [[|double(0); 1; 0|]; [|0; 0; 1|]; [|0; 0; 0|]]
        let input = Matrix<double>.Build.DenseOfArray(x)
        let result = getMatrixDerivatives input 0
        
        let y = array2D [[|double(0); 1; 1|]; [|0; 0; 1|]; [|0; 0; 0|]]
        let expected = Matrix<double>.Build.DenseOfArray(y)
        Assert.AreEqual(result, expected)

    // test to see if 'eval' function is working correctly
    [<TestMethod>]
    member this.TestMethod7 () = 
        let input = [Activation (Protein "A", Gene "b");
                                    Inhibition (Gene "b", Phenotype "*c")]
        let result = eval input

        let expected = [Inhibition (Protein "A", Phenotype "*c")]
        Assert.AreEqual(result, expected)

    // (attempt) test to see if 'testQuery' function is working properly
    [<TestMethod>]
    member this.TestMethod9 () = 
        let q = Query (Phenotype "*c", Protein "A")
        let seq = [Activation (Protein "A", Gene "b");
                                    Inhibition (Gene "b", Phenotype "*c");
                                    Query (Phenotype "*c", Protein "A")]
        let m = array2D [[|double(0); 1; 0|]; [|0; 0; 2|]; [|0; 0; 0|]]
        let M = Matrix<double>.Build.DenseOfArray(m)
        let result = testQuery q seq M
        Assert.IsTrue(true)