## JSONMonitorService

The service monitoring for if any changes in JSON file.

The service is parsing the JSON file by **Utf8JsonReader** and compare if the new JSON is changed anywhere.  

The example of the JSON dataset is taken and tested from https://www.reddit.com/r/AskReddit.json

### ENVIRONMENT

.NET 7.0

The app is written in C# and uses:
  - WPF (Windows Presentation Foundation) to interact with the service with simple UI;
  - xUnit for testing.
