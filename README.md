# 8-Bit ALU Emulator in F# ![F# Logo](https://fsharp.org/img/logo/fsharp.svg)

A functional implementation of an 8-bit arithmetic logic unit emulator demonstrating core FP concepts.

## Overview
This project implements a software emulator for basic computer arithmetic and logical operations using F#'s functional programming capabilities. Handles both signed 8-bit integers (-128 to 127) and unsigned 8-bit values (0x00 to 0xFF).

## Features
- ➕ Signed integer arithmetic: `add`, `subtract`
- 🔢 Bitwise operations: `AND`, `OR`, `XOR`, `NOT`
- 🛡️ Overflow detection for arithmetic operations
- 🔄 Bidirectional conversion between:
  - Decimal ↔ 8-bit binary
  - Signed ↔ Unsigned representations
- 🧪 Comprehensive unit tests

## Installation
1. **Requirements**:
   - [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
   - F# compiler (included with .NET SDK)
   - Recommended IDE: VS Code with Ionide extension

2. **Setup**:
    git clone https://github.com/yourusername/8bit-alu-emulator-fsharp.git
    cd 8bit-alu-emulator-fsharp
    dotnet build

## Core Functions

### Arithmetic Operations
| Function         | Signature                   | Example                    |
|------------------|-----------------------------|----------------------------|
| `add`            | int8 -> int8 -> int8 * bool| add 120y 10y → (130y, true)|
| `subtract`       | int8 -> int8 -> int8 * bool| subtract 0y (-128y) → error|

### Logical Operations
| Function         | Signature                   | Example                    |
|------------------|-----------------------------|----------------------------|
| `bitwiseAND`     | byte list -> byte list      | AND [1;0] [0;0] → [0;0]   |
| `bitwiseOR`      | byte list -> byte list      | OR [1;0] [0;1] → [1;1]    |
| `bitwiseXOR`     | byte list -> byte list      | XOR [1;0] [1;1] → [0;1]   |
| `bitwiseNOT`     | byte list -> byte list      | NOT [1;0;1] → [0;1;0]     |

### Helper Functions
- `toSigned : byte list -> int8`
- `toUnsigned : byte list -> byte`
- `detectOverflow : int -> bool`
- `validateBitLength : byte list -> Result<unit, string>`

## Implementation Highlights
### Key Functional Programming Concepts Used
1. **Tail Recursion Optimization**
   - All iterative operations use tail-recursive implementations
   - Verified using F# `[<TailCall>]` attribute

2. **Immutable Data Structures**
   - All bit manipulations return new lists rather than modifying inputs
   - Pattern matching for safe list decomposition:
     ```
     let rec processBits = function
         | [] -> []
         | head::tail -> (transform head) :: processBits tail
     ```

3. **Higher-Order Functions**
   - Heavy use of `List.map2` for bitwise operations
   - `List.fold` for binary ↔ decimal conversions


