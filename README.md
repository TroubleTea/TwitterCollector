# Thesis - Twitter Text elevation for sentiment reasoning

## Tools
    - vscode
    - anaconda
    - docker
    - docker-compose
    - postgresql

### Links

- https://docs.conda.io/en/latest/miniconda.html
- https://docs.docker.com/docker-for-windows/install/
- https://code.visualstudio.com/
- https://www.postgresql.org/docs/
- https://hub.docker.com/r/bitnami/postgresql
- https://hub.docker.com/r/bitnami/grafana/

# Libraries

## Twitter Search API
https://developer.twitter.com/en/docs/twitter-api/tweets/search/api-reference/get-tweets-search-recent

/2/tweets/search/recent

## Search operators
https://developer.twitter.com/en/docs/twitter-api/tweets/search/integrate/build-a-query#list

### Queries

#### Parameter

since_id=<last_known_tweet_id>
tweet.fields=author_id
user.fields=username

#### Get tweets

```
from:<username> OR 
from:<username> OR
from:<username> AND NOT is:reply
```

#### Get direct replies

```
is:reply AND to:<username> AND conversation_id:<tweet_id>
```

## Database

![database](database/database.png)
