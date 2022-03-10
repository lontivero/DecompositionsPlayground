let denoms = [| 10; 9; 8; 6; 5; 4; 3; 2; 1 |]

let rec Comb1 acc k (denoms: int[]) : seq<int list> =
    let acc = (denoms |> Array.head) :: acc
    if k = 0
        then seq { acc }
        else seq { 0..(denoms.Length-1) } |> Seq.collect (fun i -> C1 acc (k - 1) (denoms[i..]))

Comb1 [] 2 denoms