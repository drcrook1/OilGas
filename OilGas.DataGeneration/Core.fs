namespace OilGas.DataGeneration

module Core = 

    type FailureMessage =
        | SettingDoesNotExist
        | ConnectionTimeout
        | Exception
        | Developer
        | NotImplemented
        | UnknownDeviceIdNumber

    type Result<'TSuccess> =
        | Success of 'TSuccess
        | Failure of FailureMessage list

    type Result<'TSuccess, 'TFailure> with
        member this.SuccessValue =
            match this with
            | Success s -> s 
            | Failure _ -> Unchecked.defaultof<'TSuccess>
        member this.FailureValue =
            match this with
            | Success _ -> Unchecked.defaultof<'TFailure> 
            | Failure f -> f 

    /// Create a Success 
    let succeed x =
        Success (x)

    /// Create a Failure
    let fail x f =
        Failure f

    ///Given a function that take an input of T and returns a Result of T
    ///turns the function into a function that takes a input of Result of T
    ///and returns a Result of T
    let bind f i =
        match i with
            | Success s -> f s
            | Failure f -> Failure f
    
    ///Infix operator for bind
    let (>>=) i f =
        bind f i

    let pipeWithAdditionals r f =
        match r with
        | Success s -> r |> f
        | Failure f -> Failure f

    let (<|>) = pipeWithAdditionals

    /// given a function wrapped in a result
    /// and a value wrapped in a result
    /// apply the function to the value only if both are Success
    let applyR f result =
        match f,result with
        | Success f, Success x -> 
            f x |> Success 
        | Failure errs, Success (_) 
        | Success (_), Failure errs -> 
            errs |> Failure
        | Failure errs1, Failure errs2 -> 
            errs1 @ errs2 |> Failure 

    /// infix operator of apply
    let (<*>) = applyR

    /// given a function that transforms a value
    /// apply it only if the result is on the Success branch
    let liftR f result =
        let f' =  f |> succeed
        applyR f' result 
    
    /// infix operator of LiftR
    let (<!>) = liftR

        ///given a result and a result
    ///combine into a list of failures
    ///or a single success of the first's value
    let combineRR r1 r2=
        match r1,r2 with
        | Success s, Success s' -> s |> Success
        | Success s, Failure f -> f |> Failure
        | Failure f, Success s -> f|> Failure
        | Failure f, Failure f' -> f@f' |> Failure

    let (>*<) = combineRR
    
    ///Given a result and a function
    ///bind the result to the function
    ///return an aggregated list of failures
    ///or a success.
    let combineRF r1 f =
        let r2 = r1 >>= f
        match r1,r2 with
        | Success s, Success s' -> s |> Success
        | Success s, Failure f -> f |> Failure
        | Failure f, Success s -> f|> Failure
        | Failure f, Failure f' -> f@f' |> Failure
    
    ///Infix operator for combineRF
    let (>>=*) = combineRF