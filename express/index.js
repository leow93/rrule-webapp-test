const express = require("express");
const { rrulestr, RRule } = require("rrule");
const pg = require("pg");

const app = express();

const pool = new pg.Pool({
  max: 10,
  connectionString: "postgresql://admin:changeit@127.0.0.1:5432/rrules",
});

app.get("/instances/:from/:until", async (req, res, next) => {
  const client = await pool.connect();
  try {
    const from = new Date(req.params.from);
    const until = new Date(req.params.until);

    const { rows: rrules } = await client.query(
      `select rrule from rrules limit 1;`
    );
    const json = rrules.map((rrule) =>
      rrulestr(rrule.rrule).between(from, until, true)
    );
    res.status(200).json(json);
  } catch (e) {
    next(e);
  } finally {
    client.release();
  }
});

app.listen(3000, () => console.log("listening on port 3000"));
