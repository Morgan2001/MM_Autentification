# Authentication api

#### Build from source:

``` 
git clone https://github.com/Morgan2001/MM_Autentification
dotnet build ./src/Authentication.Api
```

#### Docker build:

```
docker build -t docker pull ghcr.io/morgan2001/mm_autentification .
```

#### Docker run:

```
docker pull docker pull ghcr.io/morgan2001/mm_autentification:master

//Development
docker run -d -p 8080:80 -e ASPNET_ENVIRONMENT=Development ghcr.io/morgan2001/mm_autentification:master

//Production 
docker run -d -p 8080:80 ghcr.io/morgan2001/mm_autentification:master
```

#### Docker compose:
```
services:
  authentication-api:
    image: ghcr.io/morgan2001/mm_autentification:master
    ports:
      - "8099:80"
    volumes:
      - /home/appsettings.json:/app/appsettings.json
```

### Api Documentation
---
OpenApi documentation is available at ```/swagger``` endpoint
