module Parser

open AST
open Combinator

let pad p = pbetween pws0 p pws0

let letterOrDigit = pletter <|> pdigit <|> (pchar '_') <|> (pchar '-')

let gene = pad (pseq plower (pmany0 letterOrDigit |>> stringify)
                           (fun (c: char, s: string) -> (string c) + s)) |>> Gene
let protein = pad (pseq pupper (pmany0 letterOrDigit |>> stringify)
                           (fun (c: char, s: string) -> (string c) + s)) |>> Protein
let phenotype = pad (pseq (pchar '*') (pmany1 letterOrDigit |>> stringify)
                           (fun (c: char, s: string) -> (string c) + s)) |>> Phenotype

let other = pad (pseq (pchar '#') (pmany1 letterOrDigit |>> stringify)
                           (fun (c: char, s: string) -> (string c) + s)) |>> Other

let variable = phenotype <|> other <|> gene <|> protein 

let activation = pseq variable (pright (pad (pstr "activates")) variable) (fun (v1,v2) -> Activation(v1, v2))
let inhibition = pseq variable (pright (pad (pstr "inhibits")) variable) (fun (v1,v2) -> Inhibition(v1, v2))
let query = pseq variable (pright (pad (pchar '?')) variable) (fun (v1,v2) -> Query(v1, v2))

let relation = activation <|> inhibition <|> query

let sequence = pmany1 (pad relation) 

let grammar = pleft sequence peof

let parse input =
    let i = prepare input
    match grammar i with
    | Success(ast,_) -> Some ast
    | Failure(_,_) -> None