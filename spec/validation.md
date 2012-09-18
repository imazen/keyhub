# License Validation

## License request from ImageResizer.dll to KeyHub (these may occur multiple times in an app lifetime on a multi-domain site).

    <licenseRequest>
    <appId>{guid}</appId>
    <domains>
      <domain name="microsoft.com">
        <feature>{guid}</feature>
        <feature>{guid}</feature>
      </domain>
    </domains>
    </licenseRequest>


## Response from KeyHub

    <licenses>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    </licenses>

In addition to creating (or retrieving) DomainLicense rows, KeyHub should log an ApplicationIssue if it failed to located a valid license for any domain/feature combination. If the ApplicationIssue has not be emailed to subscribed users in the last 2 days, send a notification.


## Decrypted license contents:
  
    Domain: microsoft.com
    Owner: Owner Name
    Issued: utc date-time
    Expires: utc date-time
    Features: {guid},{guid},{guid},{guid},{guid},{guid}
  
  
## ImageResizer 

ImageResizer will cache all provided licenses to disk, eliminating future HTTP requests until those licenses expire.
Expired licenses are automatically deleted from the cache.
  



