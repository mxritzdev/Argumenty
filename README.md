# Argumenty

Argumenty is a small argument mapper that maps flags and run arguments to models with ease.

Example: 

```sh

your executable -f input.txt --verbose

```
gets mapped to

```
YourClass(File: "input.txt", Verbose: true)
```

