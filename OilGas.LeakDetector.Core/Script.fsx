// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Library1.fs"
open OilGas.LeakDetector.Core

// Define your library scripting code here

//recursive function
let rec sum x =
    if x < 1
    then x
    else x + (sum (x - 1))

//sum 3 blows out into this
sum 3
3 + sum 2
3 + (2 + sum 1)
1 + (2 + (1 + sum 0))
3 + (2 + (1 + 0))
//results in 6

//tailed recursive function
let tailedSum(value:int) =
    let rec tailedSum' x f =
        if x < 1
        then f()
        else 
            tailedSum' (x-1) (fun () -> x + f())
    tailedSum' value (fun() -> 0)

//tailed recursive function valuation
tailedSum(3)
//Blows out into this
//tailedSum'(3, 0)
//tailedSum'(2, 1)
//tailedSum'(1, 2)
//tailedSum'(0, 3)
//6