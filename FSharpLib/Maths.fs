
module NewtonRoot
// Calculate the function value of a polynomial of type a*x^n+b*x^(n-1)+ ... + z 
// presented as a List: [ a; b; ...; z]. Coefficients a,b, ... z are floats.
// 0.0 should be used for a missing coefficient within the n+1 coefficients but any order n should be valid!
    let rec rootFV(fOrig:List<float>, length:int, root) : float  =
        match fOrig with
        | head::tail -> head*pown root (length-1) + rootFV(tail, tail.Length, root)
        | []-> 0.0

// Find derivative of original function/polynomial which is a list of Length-1, 
// where Length is the length of the original function and also its degree+1.
    let rec findDeriv(fOrig:List<float>, length:int) : List<float> =
        match fOrig with 
        |head::[]->[ ]
        |head::tail -> [head*((float length)-1.0)]@findDeriv(tail,tail.Length)

// Now let's try Newton
    open System
    let rec CNewton fOrig root err =
        let Der = findDeriv(fOrig, fOrig.Length)
        let fRoot = rootFV(fOrig, fOrig.Length, root)
        let derRoot = rootFV(Der, fOrig.Length, root)
        let nRoot = root - fRoot/derRoot
        if abs(nRoot - root) > err then
            CNewton fOrig nRoot err
        else
            nRoot
    

  

    

