package main

import (
	"context"
	"encoding/json"
	"github.com/gorilla/mux"
	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/teambition/rrule-go"
	"net/http"
	"time"
)

func getRrule(pool *pgxpool.Pool) *rrule.RRule {
	rows, err := pool.Query(context.Background(), "select rrule from rrules limit 1;")
	if err != nil {
		panic(err)
	}
	defer rows.Close()
	var result *rrule.RRule
	for rows.Next() {
		values, err := rows.Values()
		if err != nil {
			panic(err)
		}
		result, err = rrule.StrToRRule(values[0].(string))
	}
	return result
}

const YYYYMMDD = "2006-01-02"

func main() {
	connStr := "postgresql://admin:changeit@localhost:5432/rrules?sslmode=disable&pool_max_conns=10"
	pool, err := pgxpool.New(context.Background(), connStr)
	if err != nil {
		panic(err)
	}
	defer pool.Close()

	router := mux.NewRouter()

	router.HandleFunc("/instances/{from}/{until}", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		vars := mux.Vars(r)
		from, err := time.Parse(YYYYMMDD, vars["from"])
		if err != nil {
			panic(err)
		}
		until, err := time.Parse(YYYYMMDD, vars["until"])
		if err != nil {
			panic(err)
		}

		rrule := getRrule(pool)
		instances := rrule.Between(from, until, true)
		jsonBytes, err := json.Marshal(instances)
		if err != nil {
			panic(err)
		}
		w.Write(jsonBytes)
	})

	http.ListenAndServe(":3000", router)
}
