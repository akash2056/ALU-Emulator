open System
// Akash Yadav
// Hexadecimal to Binary (8-bit)
let hexToBinaryList (hex: string) : int list =
    let sanitizedHex = hex.Replace("0x", "").Replace("0X", "")
    let number = Convert.ToInt32(sanitizedHex, 16)
    let rec toBinary n acc =
        if n = 0 && List.length acc >= 8 then acc
        else toBinary (n >>> 1) ((n &&& 1) :: acc)
    let binary = toBinary number []
    List.replicate (8 - List.length binary) 0 @ binary

// Binary to Hexadecimal
let binaryListToHex (bits: int list) : string =
    let rec fromBinary bits acc =
        match bits with
        | [] -> acc
        | bit :: rest -> fromBinary rest ((acc <<< 1) ||| bit)
    sprintf "%X" (fromBinary bits 0)

// Logical Operations
let bitwiseNot = List.map (fun bit -> 1 - bit)
let bitwiseAnd = List.map2 (&&&)
let bitwiseOr = List.map2 (|||)
let bitwiseXor = List.map2 (fun x y -> if x <> y then 1 else 0)

// Decimal to Two's Complement Binary
let decimalToBinaryList (num: int) : int list =
    let byteValue = 
        if num < 0 then (~~~(abs num - 1)) &&& 0xFF
        else num &&& 0xFF
    
    let rec toBinary n acc =
        if n = 0 && List.length acc >= 8 then acc
        else toBinary (n >>> 1) ((n &&& 1) :: acc)
    
    let binary = toBinary byteValue []
    List.replicate (8 - List.length binary) 0 @ binary

// Binary Addition with Carry
let addBinaryLists (a: int list) (b: int list) : int list =
    let rec addBits aRev bRev carry acc =
        match aRev, bRev with
        | [], [] -> if carry = 1 then 1 :: acc else acc
        | aHead :: aTail, bHead :: bTail ->
            let sum = aHead + bHead + carry
            addBits aTail bTail (sum / 2) ((sum % 2) :: acc)
        | _ -> failwith "Mismatched lengths"
    addBits (List.rev a) (List.rev b) 0 [] |> List.rev

// Overflow Checking
let addWithOverflowCheck a b =
    let sum = a + b
    if sum > 127 || sum < -128 then Error "OVERFLOW" else Ok sum

let subWithOverflowCheck a b =
    let diff = a - b
    if diff > 127 || diff < -128 then Error "OVERFLOW" else Ok diff

// Enhanced Emulator with Subtraction
let rec emulator () =
    printfn "Enter operation (NOT, OR, AND, XOR, ADD, SUB, QUIT):"
    match Console.ReadLine().ToUpper() with
    | "NOT" ->
        printfn "Enter Hex value:"
        let hex = Console.ReadLine()
        try
            let bits = hexToBinaryList hex
            let result = bitwiseNot bits
            printfn "NOT %s = %s (%A)" hex (binaryListToHex result) result
        with _ -> printfn "Invalid hex"
        emulator()
    
    | "AND" | "OR" | "XOR" as op ->
        printfn "Enter first hex:"
        let hexA = Console.ReadLine()
        printfn "Enter second hex:"
        let hexB = Console.ReadLine()
        try
            let a = hexToBinaryList hexA
            let b = hexToBinaryList hexB
            let result = 
                match op with
                | "AND" -> bitwiseAnd a b
                | "OR" -> bitwiseOr a b
                | "XOR" -> bitwiseXor a b
                | _ -> []
            printfn "%s %s %s = %s (%A)" hexA op hexB (binaryListToHex result) result
        with _ -> printfn "Invalid hex"
        emulator()

    | "ADD" ->
        printfn "Enter first decimal:"
        let decA = Console.ReadLine()
        printfn "Enter second decimal:"
        let decB = Console.ReadLine()
        try
            let a = Convert.ToInt32(decA)
            let b = Convert.ToInt32(decB)
            match addWithOverflowCheck a b with
            | Ok sum ->
                printfn "Decimal: %d + %d = %d" a b sum
                printfn "Binary: %A + %A = %A" 
                    (decimalToBinaryList a) 
                    (decimalToBinaryList b)
                    (decimalToBinaryList sum)
            | Error msg -> printfn "%s" msg
        with _ -> printfn "Invalid decimal"
        emulator()

    | "SUB" ->
        printfn "Enter first decimal:"
        let decA = Console.ReadLine()
        printfn "Enter second decimal:"
        let decB = Console.ReadLine()
        try
            let a = Convert.ToInt32(decA)
            let b = Convert.ToInt32(decB)
            match subWithOverflowCheck a b with
            | Ok diff ->
                let aBits = decimalToBinaryList a
                let bBits = decimalToBinaryList b
                let bComplement = decimalToBinaryList (-b)
                
                printfn "Decimal: %d - %d = %d" a b diff
                printfn "Binary steps:"
                printfn "Minuend:    %A (%d)" aBits a
                printfn "Subtrahend: %A (%d)" bBits b
                printfn "2's compl:  %A (-%d)" bComplement b
                printfn "Sum:        %A + %A = %A" aBits bComplement (decimalToBinaryList diff)
            | Error msg -> printfn "%s" msg
        with _ -> printfn "Invalid decimal"
        emulator()

    | "QUIT" -> printfn "Exiting..."
    | _ -> 
        printfn "Invalid operation"
        emulator()

[<EntryPoint>]
let main _ =
    emulator()
    0
