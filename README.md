# Authentication api

#### Build from source:

``` 
git clone https://github.com/Morgan2001/MM_Autentification
dotnet build ./src/Authentication.Api
```

#### Docker build:

```
docker build -t Morgan2001/authentication-api .
```

#### Docker run:

```
docker run -d -p 8080:80 -e ASPNET_ENVIRONMENT=Development ghcr.io/Morgan2001/authentication-api
```

### Api Documentation
---
OpenApi documentation is available at ```/swagger``` endpoint
