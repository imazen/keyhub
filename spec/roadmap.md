#KeyHub's Roadmap to launch

# Milestones

## Preview 0 - March 1st 2013 - Private beta group

* Change all primary keys to GUID
* Solve repeat OpenID login issue
* Solve username/e-mail issue - username should always be e-mail or sha1 of email, if > 50chars
* Implement DomainLicense serialization

## Preview 1 - March 14th 3013 - All new customers

* Remove all passwords/confidential information from repository, truncate history.
* Redirect to HTTPS if request is HTTP
* Don't crash on unrecognized SKUs. Not all SKUs can be linked, this is by design.
* Add password reset support

## Preview 2 - Apr. 1st 2013 - All customers, open-sourcing begins, mostly Imazen development

* Add customer data prefill based on transaction data
* Fix private key management
* Support storage of shared secret keys for transaction REST service
* Use PayPal's list of countries. Default customer country should match the vendor's country.
* Add support for linking multiple OpenID accounts

## Preview 3 - Apr. 15th 2013 - Mild license enforcement, Imazen handles all development

* Add secure downloads section
* Implement alternate mail service
* Allow users to manage their e-mail preferences
* Add diagnostics delivery
* Add e-mail verification support


## Preview 4 - Apr 30th, 2013 - Full license enforcement

* Display special information to support contract customers
* Allow transactionitems to have notes.


---

# Sprints

#Sprint 1: Transaction import, User claiming, License creation, and App ID generation

Scenario: users are able to login, claim licenses and get the application key.

Estimated workload: 36 hours

* DONE: Extend transation REST service to accept incoming ejunky messages (both single and multi-item) and store to DB. We don't track transaction status - once the data arrives, we assume payment is complete. Entire Ejunky message will be dumped into a field (Entity Framework does not support XML field. Will be string) for later use. 
Question: what return message is required. Note that SKU's could be expired.

Transaction volume is relatively low; XML overhead negligible in this instance. Transactions between 2006 and 2012: 903. Estimated size of each: 2-4KB. I.e, < 5MB overhead.

* 2 hrs: Secure communication to transation REST service via a shared secret key in the querystring.

* 1 hrs: Set REST service to use HTTPS. Perhaps run entire KeyHub on HTTPS?

* DONE: Add OpenID login support (like NerdDinner's MVC 4 example)

http://tostring.it/2012/08/20/how-integrate-facebook-twitter-linkedin-yahoo-google-and-microsoft-account-with-your-asp-net-mvc-application/

http://nerddinner.codeplex.com/SourceControl/changeset/view/8ea1ecf71b30#mvc4%2fNerdDinner%2fControllers%2fAccountController.cs

* DONE: Issue #12: Add extra field FeatureName to Feature. Add validation on editing FeatureCode to be GUID and unique. FeatureID will be primary key and cannot be changed.

* DONE: Issue #11: When claiming license automatically create an application and application key if none exist. Show application key to user as key to add to web.config, as seen in validation.md.

* DONE: Implement business rules to set License expiration date based on SKU properties.

* DONE: Implement business rules to follow SKU expiration date. Integrate check into the transaction REST service. Provides error message back if SKU has expired. SKU will not be part of the transaction.

* DONE: Issue #10: CountryCode on Checkout

* DONE: Issue #9: department field optional

* 5 hrs (optional): Implement e-mail sending for received transactions and for manual 're-send' requests based on txn id or payer_email.
* Update content of country list during initial DB creation to match with PayPall countries

#Sprint 2: License validation

Scenario: Vendors will be able to generate (encrypted) private keys and export/view the public key xml. Libraries will be able to request licenses from KeyHub (see Validation.md) and validate them. KeyHub will store the domain licenses it generates for future requests.

* REST service license validation and encrypted licenses response.

* Issue #11: Domains will be automatically added upon license validation. Domain duration will be based on SKU properties. Allow a way to delete unused domains. Do we actually need a way to add or edit a domain? Answer: yes, but manual domain creation/deletion can be stage 3.

* Implement business rule to manage max allowed domains per license.

* Allow vendors to manage the SKU private Key Bytes.


#Sprint 3: Adding download links, Extending usability

Scenario: KeyHub supports the more advanced users supporting additional accounts and smoother transaction handling

* Keyhub displays secured download links for each licensed product - can be provided via XML feed to KeyHub or through the DB.

* Allow way to create a new application key when previous is compromised.

* Allow manual (permanent) license generation for 'offline ode'

* Finalize creation and management of user accounts.

* Add additional ejunky fields to transaction. Automatically fill in customer fields or  even generate licenses based on incoming message.

* Allow manual domain creation/deletion


#Sprint 4: ApplicationIssues, Notification & extended features.

Scenario: Licesnse validation fails - KeyHub is responsible for sending notification e-mails to the appropriate recipients.
Scenario: ImageResizer has issues on the diagnostics page (or is running a vulnerable version) - if configured, they will be sent to KeyHub, and e-mailed to all subscribed users.


# Backlog

The backlog is a list of to-do items that have not yet been assigned to a sprint/stage.

1. Logged in users will be shown a dashboard, containing a list of their Applications and the number/severity of issues. Clicking an application will show its issues.
3. Must be deployed in replicated configuration with DB backups
4. E-mail validation to enable e-mail based claims
6. Password reset ability
7. Link/unlink openID

# Nathanael's Backlog

These are features I wish to add, but are outside the agreed scope

1. Generate/display PDF invoice for a transaction. Assigned to Nathanael (not within agreement scope).
2. Display upgrade cupon codes based on past purchases
3. Display support contact information to contract holders.
4. Display unused support incident information.
5. Add support for old-style download link imports.
6. Add support for multiple public keys
7. Add newsletter/updates subscription checkbox




