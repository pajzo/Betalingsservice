# Betalingsservice

![Build and pack](https://github.com/pajzo/Betalingsservice/workflows/Build%20and%20pack/badge.svg?branch=master)

Methods to parse and write files for Betalingsservice

## Parse

### 602 Payment info
```csharp
var paymentInfo = ParserClient.GetPaymentInformation(lines);
```

### 603 Mandate info
```csharp
var mandateInfo = ParserClient.GetMandateInformation(lines);
```

### 621 Information
```csharp
var informationEvents = ParserClient.GetInformationEvents(lines);
```
