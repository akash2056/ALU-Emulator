open System

// Shared Helper Functions
let formatBinary bits = 
    sprintf "[%s]" (String.concat "; " (List.map string bits))

let binaryOutputFormat binary =
    binary |> Seq.map string |> String.concat ";"

// Logical Operations
let hexToBinaryList (hex: string) : int list =
    let sanitizedHex = hex.Replace("0x", "").Replace("0X", "")
    let number = Convert.ToInt32(sanitizedHex, 16)
    [for i in 7..-1..0 -> (number >>> i) &&& 1]

let binaryListToHex (bits: int list) : string =
    bits
    |> List.fold (fun acc bit -> (acc <<< 1) ||| bit) 0
    |> sprintf "%02X"

let rec logicalEmulator () =
    printfn "\n=== Logical Operations ==="
    printfn "Available operations: NOT, AND, OR, XOR, BACK"
    printfn "Enter operation:"
    match Console.ReadLine().ToUpper() with
    | "NOT" ->
        printf "Enter Hex value: "
        let hex = Console.ReadLine()
        try
            let bits = hexToBinaryList hex
            let result = List.map (fun b -> 1 - b) bits
            printfn "\nNOT %s:\n%s = %s" hex (formatBinary result) (binaryListToHex result)
        with _ -> printfn "Invalid hex input"
        logicalEmulator()
    
    | "AND" | "OR" | "XOR" as op ->
        printf "Enter first Hex value: "
        let hexA = Console.ReadLine()
        printf "Enter second Hex value: "
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
        with _ -> printfn "Invalid hex input"
        logicalEmulator()

    | "BACK" ->
        printfn "Returning to main menu..."
        ()

    | _ ->
        printfn "Invalid operation. Valid operations: NOT, AND, OR, XOR, BACK"
        logicalEmulator()

// Arithmetic Operations
let binaryAddition (a: string) (b: string) : string =
    let mutable carry = 0
    let result = Text.StringBuilder()
    for i in 7..-1..0 do
        let bitA = int (a.[i].ToString())
        let bitB = int (b.[i].ToString())
        let sum = bitA + bitB + carry
        carry <- sum / 2
        result.Insert(0, (sum % 2).ToString()) |> ignore
    result.ToString()

let binarySubtraction (a: string) (b: string) : string =
    let twosComplement (binary: string) : string =
        let inverted = binary |> Seq.map (fun c -> if c = '0' then '1' else '0') |> String.Concat
        let result = Text.StringBuilder()
        let mutable carry = 1
        for i in 7..-1..0 do
            let bit = int (inverted.[i].ToString()) + carry
            carry <- bit / 2
            result.Insert(0, (bit % 2).ToString()) |> ignore
        result.ToString()
    binaryAddition a (twosComplement b)

let convertIntTo8BitBinary (num: int) : string =
    if num < -128 || num > 127 then
        raise (ArgumentOutOfRangeException("Number must be between -128 and 127"))
    Convert.ToString(num &&& 0xFF, 2).PadLeft(8, '0')

let convertBinaryToInt (bin: string) : int =
    if bin.[0] = '1' then Convert.ToInt32(bin, 2) - 256
    else Convert.ToInt32(bin, 2)

let tryParseSByte (input: string) =
    match Int32.TryParse(input) with
    | (true, num) when num >= -128 && num <= 127 -> Ok num
    | (true, _) -> Error "Number out of range (-128 to 127)"
    | _ -> Error "Invalid decimal input"

let rec arithmeticEmulator () =
    printfn "\n=== Arithmetic Operations ==="
    printfn "Available operations: ADD, SUB, BACK"
    printf "Enter operation: "
    match Console.ReadLine().ToUpper() with
    | "ADD" | "SUB" as op ->
        printf "Enter first decimal (-128 to 127): "
        let in1 = Console.ReadLine()
        printf "Enter second decimal (-128 to 127): "
        let in2 = Console.ReadLine()

        match tryParseSByte in1, tryParseSByte in2 with
        | Ok num1, Ok num2 ->
            let nb1 = convertIntTo8BitBinary num1
            let nb2 = convertIntTo8BitBinary num2
            
            let binaryResult = 
                match op with
                | "ADD" -> binaryAddition nb1 nb2
                | "SUB" -> binarySubtraction nb1 nb2
                | _ -> failwith "Invalid operation"

            let decResult = convertBinaryToInt binaryResult
            
            // Convert binary strings to int lists for formatting
            let bits1 = nb1 |> Seq.map (fun c -> int c - int '0') |> List.ofSeq
            let bits2 = nb2 |> Seq.map (fun c -> int c - int '0') |> List.ofSeq
            let resultBits = binaryResult |> Seq.map (fun c -> int c - int '0') |> List.ofSeq

            // Modified output to match logical operations format
            printfn "\n%d %s %d:" num1 op num2
            printfn "%s %s" (formatBinary bits1) op 
            printfn "%s =" (formatBinary bits2)
            printfn "%s = %d" (formatBinary resultBits) decResult
        
        | Error e, _ -> printfn "Error: First input - %s" e
        | _, Error e -> printfn "Error: Second input - %s" e
        arithmeticEmulator()

    | "BACK" ->
        printfn "Returning to main menu..."
        ()

    | _ ->
        printfn "Invalid operation"
        arithmeticEmulator()

// Main Program
let rec mainMenu () =
    printfn "\n=== ALU Emulator ==="
    printfn "Select operation mode:"
    printfn "1. LOGICAL (NOT/AND/OR/XOR)"
    printfn "2. ARITHMETIC (ADD/SUB)"
    printfn "3. QUIT"
    printf "Your choice: "
    
    match Console.ReadLine().ToUpper() with
    | "LOGICAL" | "1" -> logicalEmulator(); mainMenu()
    | "ARITHMETIC" | "2" -> arithmeticEmulator(); mainMenu()
    | "QUIT" | "3" -> printfn "Exiting ALU emulator..."
    | _ -> printfn "Invalid selection"; mainMenu()

[<EntryPoint>]
let main _ =
    mainMenu()
    0
