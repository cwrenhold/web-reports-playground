package initializers

import (
	"fmt"
	"log"
	"os"

	"github.com/jmoiron/sqlx"
	_ "github.com/lib/pq"
)

var SqlxDB *sqlx.DB

func ConnectToDB() {
	host := os.Getenv("POSTGRES_SERVER")
	port := os.Getenv("POSTGRES_PORT")
	user := os.Getenv("POSTGRES_USER")
	password := os.Getenv("POSTGRES_PASSWORD")
	dbname := os.Getenv("PROJECT_DB")

	var err error
	dsn := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=disable TimeZone=Europe/London", host, user, password, dbname, port)
	SqlxDB, err = sqlx.Connect("postgres", dsn)

	if err != nil {
		log.Fatal("failed to connect database")
	}
}
