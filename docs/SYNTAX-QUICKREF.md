# Codex Syntax Quick Reference

For agents. Read this before writing test sources or samples.

## Definitions

```
square : Integer -> Integer
square (x) = x * x

add : Integer -> Integer -> Integer
add (x) (y) = x + y
```

## Records

```
Person = record {
  name : Text,
  age : Integer
}

get-name : Person -> Text
get-name (p) = p.name

main : Text
main = get-name (Person { name = "Alice", age = 30 })
```

## Sum Types (Variants)

```
Shape =
  | Circle (radius : Integer)
  | Rect (width : Integer) (height : Integer)
```

## Pattern Matching

```
describe : Shape -> Text
describe (s) =
  when s
    if Circle (r) -> "circle"
    if Rect (w) (h) -> "rect"
```

`when`/`if` — not `match`/`case`. Wildcard: `if _ -> ...`

## Effects

```
main : [Console] Nothing
main = print-line "hello"

greet : Text -> [Console, FileSystem] Nothing
greet (name) = do
  contents <- read-file "template.txt"
  print-line (contents ++ name)
```

## Do Notation

```
main : [Console] Nothing
main = do
  name <- read-line
  print-line ("Hello, " ++ name)
```

## If/Then/Else

```
abs : Integer -> Integer
abs (n) = if n < 0 then negate n else n
```

## Let Bindings

```
main : Integer
main = let x = 10 in let y = 20 in x + y
```

## Booleans

`True` / `False` — capital T/F.

## Operators

Arithmetic: `+` `-` `*` `/`
Comparison: `==` `!=` `<` `>` `<=` `>=`
Boolean: `&&` `||`
Text concat: `++`

## Effect Declarations

```
effect Console where
  print-line : Text -> [Console] Nothing
  read-line  : [Console] Text
```

## Linear Types

```
open-file : Text -> [FileSystem] linear FileHandle
close-file : linear FileHandle -> [FileSystem] Nothing
```
