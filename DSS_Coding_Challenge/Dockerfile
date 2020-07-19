FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY bin/Release/netcoreapp3.1/publish/ App/
COPY sample_input/ App/sample_input/
COPY sample_output/ App/sample_output/
WORKDIR /App
ENTRYPOINT ["dotnet", "DSS_Coding_Challenge.dll"]
