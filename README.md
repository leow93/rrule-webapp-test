# RRule Webapp tests

## Seeding data:
```shell
cd express
nvm use 
pnpm i
node seed.js
```

## To run the load test
```shell
k6 run --vus 10 --duration 10s ./test.js
```

## Express 
To run with express:
```shell
cd express
node index.js
```

## F#
```shell
cd fsharp 
dotnet run --project=.
```

# Goland
```shell
cd golang
go run main.go
```