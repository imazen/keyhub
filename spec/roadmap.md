#KeyHub's Roadmap to launch

Estimation of hours and planning of items will be added shortly


#Stage 1: License creation and management

Scenario: users are able to login, claim licenses and get the application key.


* Implement mediator between Ejunky and KeyHub to accept incoming ejunky messages (both single and multi-item) and push to KeyHub. At first stage only filter out parts needed. See stage 3 for later additions. 
Note: KeyHub does not track transaction status. License is created upon Ejunky message and users can start using the license.
How is communication secured?

* Issue #12: Add extra field FeatureName to Feature. Add validation on editing FeatureCode to be GUID and unique. FeatureID will be primary key and cannot be changed.

* Issue #11: When claiming license automatically create an application and application key. Show application key to user as key to add to web.config. Will the license tags in web.config be filled in based on validation result??  

* Implement business rules to set License expiration date based on SKU properties.

* Issue #10: CountryCode

* Issue #9: department field optional


#Stage 2: License validation

Scenario: libraries will be able to automatically validate licenses through the KeyHub REST service. KeyHub wil automatically track registered domains.

* REST service license validation and encrypted licenses response.

* Add private key bytes to SKU.

* Issue #11: Domains will be automatically added upon license validation. Allow a way to delete unused domains. Do we actually need a way to add or edit a domain?

* Implement business rule to manage max allowed domains per license.

* Allow vendors to manage the SKU private Key Bytes.


#Stage 3: Extending usability

Scenario: KeyHub supports the more advanced users supporting additional accounts and smoother transaction handling

* Allow way to create a new application key when previous is compromised.

* Finalize creation and management of user accounts.

* Add additional ejunky fields to transaction. Automatically fill in customer fields or  even generate licenses based on incoming message.


#Stage 4: ApplicationIssues, Notification & extended features.

Scenario: KeyHub tracks and communicates warnings and events.
