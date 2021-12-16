# Decompositions Playground (with matrix)

We want to decompose a number is a way to represent it as a sum of smaller "standard" numbers. Let imagine we have a number $n$ where $n$ ∈ (0, 10] and we want to decompose it in a way that the sum of the decompositions is equal to $n$.

Also, only the following "standard" numbers are allowed: `1, 2, 3, 4, 5, 6, 8, 9, 10`.

## Decomposition matrix

The idea is to decompose a number as the sum of at most $k$ smaller "standard" numbers. See the following matrix for an example of decomposition of number $7$ with $k$ = 5:

```
{ 6, 1, 0, 0, 0 }
{ 5, 2, 0, 0, 0 }
{ 5, 1, 1, 0, 0 }
{ 4, 3, 0, 0, 0 }
{ 4, 2, 1, 0, 0 }
{ 4, 1, 1, 1, 0 }
{ 3, 3, 1, 0, 0 }
{ 3, 2, 2, 0, 0 }
{ 3, 2, 1, 1, 0 }
{ 3, 1, 1, 1, 1 }
{ 2, 2, 2, 1, 0 }
{ 2, 2, 1, 1, 1 }
```

```
{ { 6, 1 }  }
{ { 5, 2 }, { 5, 1, 1 }  }
{ { 4, 3 }, { 4, 2, 1 }, { 4, 1, 1, 1 }  }
{ {  ,   }, { 3, 3, 1 }, { 3, 2, 2, 0 }, { 3, 2, 1, 1 }, { 3, 1, 1, 1, 1 } }
{ {  ,   }, {  ,  ,   }, {  ,  ,  ,   }, { 2, 2, 2, 1 }, { 2, 2, 1, 1, 1 } }
