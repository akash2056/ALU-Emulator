open System

//Akash Yadav
//Helper function for binary formatting
let formatBinary bits = 
    sprintf "[%s]" (String.concat "; " (List.map string bits))

//Hexadecimal to Binary (8-bit)
let hexToBinaryList (hex: string) : int list =
    let sanitizedHex = hex.Replace("0x", "").Replace("0X", "")
    let number = Convert.ToInt32(sanitizedHex, 16)
    [for i in 7..-1..0 -> (number >>> i) &&& 1]

//Binary to Hexadecimal
let binaryListToHex (bits: int list) : string =
    bits
    |> List.fold (fun acc bit -> (acc <<< 1) ||| bit) 0
    |> sprintf "%02X"

//Decimal to Two's Complement Binary (8-bit signed integers)
let decimalToBinaryList (num: int) : int list =
    let byteValue = 
        if num < 0 then (~~~(abs num - 1)) &&& 0xFF
        else num &&& 0xFF
    [for i in 7..-1..0 -> (byteValue >>> i) &&& 1]

//Binary Addition with Carry Visualization
let addBinaryLists (a: int list) (b: int list) : int list * string list =
    let rec addBits aRev bRev carry acc steps =
        match aRev, bRev with
        | [], [] -> 
            let final = if carry = 1 then 1 :: acc else acc
            (List.rev final, List.rev steps)
        | aHead :: aTail, bHead :: bTail ->
            let sum = aHead + bHead + carry
            let newCarry = sum / 2
            let step = 
                sprintf "Bit %d: %d + %d + %d = %d (Carry %d)" 
                    (List.length acc) aHead bHead carry (sum % 2) newCarry
            addBits aTail bTail newCarry ((sum % 2) :: acc) (step :: steps)
        | _ -> failwith "Mismatched lengths"
    addBits (List.rev a) (List.rev b) 0 [] []

//Overflow Checking for Signed Integers (-128 to 127)
let checkOverflow result =
    if result > 127 || result < -128 then true else false

//Emulator with Formatted Output and Overflow Handling
let rec emulator () =
    printfn "\nEnter operation you want to perform (NOT, OR, AND, XOR, ADD, SUB or QUIT):"
    match Console.ReadLine().ToUpper() with
    | "NOT" ->
        printfn "Enter Hex value:"
        let hex = Console.ReadLine()
        try
            let bits = hexToBinaryList hex
            let result = List.map (fun b -> 1 - b) bits
            printfn "\nResult of NOT %s:\n%s = %s" hex (formatBinary result) (binaryListToHex result)
        with _ -> printfn "Invalid hex"
        emulator()
    
    | "AND" | "OR" | "XOR" as op ->
        printfn "Enter first Hex value:"
        let hexA = Console.ReadLine()
        printfn "Enter second Hex value:"
        let hexB = Console.ReadLine()
        try
            let a = hexToBinaryList hexA
            let b = hexToBinaryList hexB
            let result = 
                match op with
                | "AND" -> List.map2 (&&&) a b
                | "OR" -> List.map2 (|||) a b
                | "XOR" -> List.map2 (fun x y -> if x <> y then 1 else 0) a b
                | _ -> []
            printfn "\n%s %s %s:\n%s %s\n%s =\n%s = %s" 
                hexA op hexB
                (formatBinary a) op 
                (formatBinary b)
                (formatBinary result)
                (binaryListToHex result)
        with _ -> printfn "Invalid hex"
        emulator()

    | "ADD" ->
        printfn "Enter first decimal value:"
        let decA = Console.ReadLine()
        printfn "Enter second decimal value:"
        let decB = Console.ReadLine()
        try
            let a = Convert.ToInt32(decA)
            let b = Convert.ToInt32(decB)
            if checkOverflow(a + b) then 
                printfn "\nOverflow detected! Result exceeds signed 8-bit range (-128 to 127)."
            else
                let aBits = decimalToBinaryList a
                let bBits = decimalToBinaryList b
                let sumBits, steps = addBinaryLists aBits bBits
                
                printfn "\nBinary Addition Steps:"
                printfn "A: %s" (formatBinary aBits)
                printfn "B: %s" (formatBinary bBits)
                steps |> List.iter (printfn "%s")
                printfn "\nFinal Result:"
                printfn "%s + %s =\n%s = %d (%s)"
                    (formatBinary aBits)
                    (formatBinary bBits)
                    (formatBinary sumBits)
                    (a + b)
                    (binaryListToHex sumBits)
        with _ -> printfn "Invalid decimal"
        emulator()

    | "SUB" ->
        printfn "Enter first decimal value:"
        let decA = Console.ReadLine()
        printfn "Enter second decimal value:"
        let decB = Console.ReadLine()
        try
            let a = Convert.ToInt32(decA)
            let b = Convert.ToInt32(decB)
            if checkOverflow(a - b) then 
                printfn "\nOverflow detected! Result exceeds signed 8-bit range (-128 to 127)."
            else 
                let aBits = decimalToBinaryList a
                let bBits = decimalToBinaryList b
                
                // Two's complement of subtrahend for subtraction operation
                let bComplement =
                    bBits |> List.map (fun bit -> 1 - bit)
                          |> fun inv -> fst(addBinaryLists inv [0;0;0;0;0;0;0;1])
                
                // Perform addition of minuend and two's complement of subtrahend
                let sumBits, steps = addBinaryLists aBits bComplement
                
                printfn "\nBinary Subtraction Steps:"
                printfn "Minuend:    %s (%d)" (formatBinary aBits) a
                printfn "Subtrahend: %s (%d)" (formatBinary bBits) b
                printfn "2's compl:  %s (-%d)" (formatBinary bComplement) b
                steps |> List.iter (printfn "%s")
                
                printfn "\nFinal Result:"
                printfn "%s - %s:\n%s\n= %d (%s)"
                    (formatBinary aBits)
                    (formatBinary bComplement)
                    (formatBinary sumBits)
                    (a - b)
                    (binaryListToHex sumBits)
        with _ -> printfn "Invalid decimal"
        emulator()

    | "QUIT" ->
        printfn "\nExiting program..."
    
    | _ ->
        printfn "\nInvalid operation. Try again."
        emulator()

[<EntryPoint>]
let main _ =
    emulator()
    0
