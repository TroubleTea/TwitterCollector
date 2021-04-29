CREATE SCHEMA IF NOT EXISTS sentiment;
-- ALTER SCHEMA sentiment OWNER TO sentiment;

SET search_path to sentiment;
CREATE TABLE IF NOT EXISTS twitter_users(
  id BIGINT NOT NULL PRIMARY KEY,
  full_name TEXT NOT NULL,
  twitter_handle TEXT,
  party TEXT,
  constituency TEXT
);
-- ALTER TABLE twitter_users OWNER TO sentiment;

CREATE TABLE IF NOT EXISTS jobs(
  id SERIAL PRIMARY KEY,
  job_name TEXT NOT NULL,
  started_at TIMESTAMP NOT NULL DEFAULT current_timestamp,
  finished_at TIMESTAMP,
  total_tweets_imported INTEGER,
  rate_limit_reached BOOLEAN
);
-- ALTER TABLE jobs OWNER TO sentiment;

CREATE TABLE IF NOT EXISTS tweets(
  id BIGINT NOT NULL PRIMARY KEY,
  text TEXT NOT NULL,
  tweeted_at TIMESTAMP NOT NULL,
  imported_at TIMESTAMP NOT NULL DEFAULT current_timestamp,
  author_id BIGINT NOT NULL,
  sentiment DOUBLE PRECISION,
  conversation_id BIGINT,
  is_reply BOOLEAN DEFAULT FALSE,
  job_id INT NOT NULL REFERENCES jobs(id)
);
-- ALTER TABLE tweets OWNER TO sentiment;
