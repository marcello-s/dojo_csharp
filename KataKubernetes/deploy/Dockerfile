FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# copy sources and restore project files
COPY . ./
RUN dotnet restore

# publish
RUN dotnet build /app/KataKubernetes.slnx && dotnet publish /app/KataKubernetes.csproj --configuration Release --output /out

# build runtime image
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app
COPY --from=build-env /out /app

ENV ASPNETCORE_URLS=http://+:5102
EXPOSE 5102
ENTRYPOINT ["dotnet", "KataKubernetes.dll"]