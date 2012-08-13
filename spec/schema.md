# Data model

`under construction`

### Diagram

![Schema Diagram](http://www.gliffy.com/pubdoc/3749127/L.png)
[View diagram here](http://www.gliffy.com/go/publish/3749127/)
[Edit diagram here](http://www.gliffy.com/gliffy/#d=3749127&t=KeyHub_Schema)


### Sample transactions

https://docs.google.com/spreadsheet/ccc?key=0AgBQf0FS96bPdG5DM0hBTnA1cmd6UXBCc1F4aF9YMHc


#### Abbreviations

* str256 = nvarchar(256)
* str1k = nvarchar(1024)
* str4k = nvarchar(4096)
* fk = foreign key
* pk = primary key)
* n = nullable


### Notes

All date/time values are in UTC
All date/time columns will be in 'new' style (date, time, datetime2)

## Data Model 

### SKUs

* pk id
* fk private_key_id
* fk vendor_id
* str256 sku
* n int max_domains - The maximum number of domain licenses permitted by this license
* n int edit_ownership_duration - How long the Owner_* fields are editable after license is issued
* n int max_support_contacts - Maxmimum number of users listed as a support contact
* n int change_support_contacts_duration - How long the assigned support contact can be changed
* n int license_duration - How long the license is valid for
* n int auto_domain_duration - How long auto-generated domain licenses are valid before they must be auto-renewed
* n int manual_domain_duration - How long manually generated domain licenses are valid for
* bool can_delete_manual_domains - If true, users can delete manual licenses to make room for more or do cleanup
* bool can_delete_auto_domains - If true, users can delete auto-generated licenses to make room for more or do cleanup
* n datetime2 release_date - When this SKY is first offered for purchase
* n datetime2 expiration_date - When this SKU is no longer available for purchase


### FeatureIDs

Features are abilities unlocked by a license for a given SKU. 

* pk id (guid)
* fk vendor_id
* str1k display_name

### SKUFeatureIDs

Many-to-many relationship between SKUs and FeatureIDs

* fk sku_id
* fk feature_id

### PrivateKeys

* pk id
* fk vendor_id
* str1k display_name
* binary(4096) private_key_bytes (encrypted by password in web.config)


### Vendors

* pk id (guid)
* str1k name
* street/city/region/postal/country
* str1k billing_email

### Rights

* pk id (guid)
* str1k display_name

#### Vendor rights:

* VendorAdmin
* VendorReporting

####Entity rights:

* BelongToEntity (no rights)
* EditEntityInfo
* EditEntityMembers
* GrantUsersEntityRights

##### License and/or Entity Rights

* ViewLicenseInfo (incl. transaction data)
* EditLicenseInfo
* ViewApps
* ViewAppData (enables notifications)
* EditApps
* ViewDomains
* EditDomains
* GrantUsersLicenseRights

### UserObjectRights

Connects Users to Objects (Entities, Licenses, Vendors) with Rights

* pk id
* fk user_id (guid)
* fk right_id (guid)
* fk object_id (guid)
* object_type (?)


### Users

* pk id (guid)
* str1k name
* str1k email
* bool IsEmailConfirmed (permits display of unclaimed transactions matching the e-mail address)

* //TODO: OpenID integration fields and e-mail address validation fields

### Licenses

* pk id
* fk sku_id
* fk transaction_item_id
* str owner_name
* fk purchasing_entity_id
* fk owning_entity_id
* datetime2 issued
* n datetime2 expires

### Apps

* pk id (guid)
* str1k display_name

### AppKeys

App keys can be created and deleted, but not edited. Simple. Their guid ID serves as their identifier. 

* pk id (guid)
* fk app_id


### AppLicenses

The interface must prevent an application from deleting its last license, as it would become orphaned.
Applications can have multiple licenses, and licenses can be used on multiple applications.

* fk license_id
* fk app_id

### DomainLicenses

Both automatic and manual licenses keys are stored here. 
For simplicity, we do not differentiate between domains and subdomains. In the .dll, we automatically strip "www." off domains, but other subdomains will require separate licenses.

* pk id
* fk license_id
* str4k domain_name
* datetime2 issued - The date the row was automatically or manually created
* n datetime2 expires - Manually created licenses may or may not have an expiration date. If there is no expiration, it should be null.
* bool is_automatic - Should be true if automatically generated
* binary(4096) license_bytes


### Packages

* pk id
* fk vendor_id
* str1k display_name

### PackageReleases

As an alternative to this table, Packages could simply reference an RSS XML feed... 

* pk id
* fk package_id
* str1k display_name - Will usually be something like "Resizer 4 alpha 1"
* str1k version - The official version, like "4.0.1"
* int stability - A stability rating, where 0 is stable, 1 is RC, 2 is beta, 3 is alpha, 4 is preview, etc.
* str4k notes_url - URL of release notes
* str4k download_url - URL of download. May need to be signed for public access if private on S3. 
* datetime2 release_date - When this release becomes visible

### SKUPackages

* pk id
* fk package_id
* fk sku_id

### AppIssues

* pk id
* fk app_id
* str Severity
* str2 Message
* str2 Details
* datetime2 time

### AppStats

* pk id
* fk app_id
* datetime2 time
* int stat_kind
* bigint stat_value

### AppNotificationKind

* pk id (guid)
* str display_name Ex. LicensingFailure, InsecureVersion, OutdatedVersion, CriticalErrors, Errors, Warnings

### UserAppNotifications

* pk id
* fk user_id
* fk app_id
* fk app_notification_kind_id

### Transaction Overview

While e-commerce systems generally use a 'quanity' field instead of splitting each item into a separate row, this would complicate evaluation of claimed/unclaimed transaction items. 

Unless anyone objects, it seems that splitting these apart during import will simplify all later phases.

To see what data e-junkie will be sending over HTTP, see http://www.e-junkie.com/ej/help.integration.htm

Existing data will be provided in UTF-8 form with a byte-order mark, tab delimited format. 

Example of export format: https://docs.google.com/spreadsheet/ccc?key=0AgBQf0FS96bPdG5DM0hBTnA1cmd6UXBCc1F4aF9YMHc


### TransactionItems table

* pk id (guid)
* n fk sku_id  (nullable, because it's possible that we won't be able to match it up to an SKU)
* fk transaction_id
* n float gross (Gross sale price of transaction item. Will need to be divided by quantity during import)

The following fields are recorded, but not displayed unless SKU lookup fails. These can be stored in xml or plain columns, whichever is preferred. These fields may change with e-commerce providers

* str1k sku 
* str1k item_name 
* str1k item_number
* n option_name1 - (if applicable) if you are using any options with your products, then this will contain first option's name
* n option_selection1 - (if applicable) if you are using any options with your products, then this will contain first option's value that buyer selected
* n option_name2
* n option_selection2
* n option_name3
* n option_selection3



Most e-commerce packages do not permit you to specify a different HTTP POST URL for each product, which means KeyHub will need to be able to store and tolerate transactions which cannot map to any SKU.


### Transactions

* pk id (guid)
* datetime2 payment_date
* str1k payer_email
* str1k first_name
* str1k last_name

`in progress`

We may want to store all the original data in an XML column and only keep certain fields in the schema. 

`In progress`

Similar to PayPal IPN. See http://www.e-junkie.com/ej/help.integration.htm

### Example transaction post.

    payment_date=02%3A51%3A26+Jul+18%2C+2012+MST
	payer_email=angelagube%40photometer.com
	address_name=
	address_state=
	address_country=
	address_city=
	address_zip=
	address_street=
	address_country_code=
	first_name=Kathrin
	last_name=Fretz
	payer_id=XH947JE7P6KRJ
	residence_country=CH
	payer_status=unverified
	invoice=wc8dv152xb70684oguh3n7ds0scsg0oc0okko0
	address_status=
	payer_business_name=
	payer_phone=
	custom=
	mc_currency=USD
	business=billing%40imazen.co
	mc_gross=249
	mc_shipping=0.00
	tax=0.00
	item_name1=Resizer+3
	item_number1=929356%7C%7CR3Bundle1Ent
	mc_gross_1=249
	quantity1=1
	num_cart_items=1
	pending_reason=None
	txn_id=6MF63406U4306732Y
	payment_status=Completed
	txn_type=expresscheckout
	payment_type=instant
	mc_fee=9.02
	mailing_list_status=true
	client_id=41912
	item_name=Resizer+3
	item_number=929356%7C%7CR3Bundle1Ent
	quantity=1
	option_name1=
	option_selection1=
	option_name2=
	option_selection2=
	option_name3=
	option_selection3=
	charset=utf-8
	charset_assumed=true
	buyer_ip=195.65.234.10
	handshake=ff35a320762dcec799d9c0bb9831577c
	discount_codes=
	from_name=Imazen
	from_email=billing%40imazen.co
	mailing_list_status=true
	client_shipping_method_id=0
	item_cart_position=1
	sku=R3Bundle1Ent

	ej_txn_id=14521369


### E-junkie TransactionItem

	str item_name
	int item_number - item number you have set in product configuration
	quantity - quantity sold
	mc_gross - sale price for this product * quantity sold
	option_name1 - (if applicable) if you are using any options with your products, then this will contain first option's name
	option_selection1 - (if applicable) if you are using any options with your products, then this will contain first option's value that buyer selected
	option_name2
	option_selection2
	option_name3
	option_selection3


### Current Transaction Schema (1 row per transaction item):


Payment Date (MST)	Processed by E-j (MST)	Transaction ID	Payment Processor	E-j Internal Txn ID	Payment Status	First Name	Last Name	Payer E-mail	Billing Info	Payer Phone	Card Last 4	Card Type	Payer IP	Passed Custom Param.	Discount Codes	Invoice	Shipping Info	Shipping Phone	Shipping	Tax	eBay Auction Buyer ID	Affiliate E-mail	Affiliate Name	Affiliate ID		Currency	Item Name	Variations/Variants	Item Number	SKU	Quantity	Amount	Affiliate Share (per item)	Download Info	Key/Code (if any)	eBay Auction ID	Buyer Country
6/8/11 2:24	6/8/11 2:24	gc-363976288355594	Google Checkout	9476313	Completed	Andrew	R Ward	Andrew-bjk6pjj1dmn@checkout.google.com	45 Whitfield St, London, LONDON, W1T4HD, United Kingdom (Great Britain)				83.244.237.98		For Cart Item Total: 60OFFLOYALTY, 	3.63976E+14	Andrew R Ward, 45 Whitfield St, London, LONDON, W1T4HD, United Kingdom (Great Britain)		0	0					0	USD	Resizer 3	Bundle:Performance Bundle, License:Pro License	929356	R3Bundle1Pro	1	39.6	0	1 attempt(s), Last by 83.244.237.98 @ 2011-06-08 02:24:23			United Kingdom (Great Britain)

