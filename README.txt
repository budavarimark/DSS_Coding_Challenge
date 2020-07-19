A DSS_Coding_Challenge mappába belépve parancssorból:

Alapból:
bin\Release\netcoreapp3.1\DSS_Coding_Challenge.exe sample_input/sample1.csv sample_output/
vagy
dotnet run sample_input/sample1.csv sample_output/


Docker:
docker build -t dss-image -f Dockerfile .
docker run -it --rm dss-image sample_input/sample1.csv sample_output/
