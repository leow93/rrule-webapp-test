const pg = require("pg");
const { RRule, datetime, rrulestr } = require("rrule");
const crypto = require("node:crypto");

const rrule = new RRule({
  freq: RRule.DAILY,
  dtstart: datetime(2023, 1, 1, 0, 0),
})

const main = async () => {
  const client = new pg.Client(
    "postgres://admin:changeit@127.0.0.1:5432/rrules"
  );
  await client.connect();
  await client.query(
    `
  create table if not exists rrules(
      id uuid not null,
      rrule text not null,
      primary key(id)
  )
`
  );

  await client.query("BEGIN;");
  await client.query("delete from rrules;");
  await client.query("insert into rrules (id, rrule) values ($1, $2)", [
    crypto.randomUUID(),
    rrule.toString(),
  ]);
  await client.query("COMMIT");

  client.end();
};

void main();
