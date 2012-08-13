# Data model

`under construction`

### Diagram

![Schema Diagram](http://www.gliffy.com/pubdoc/3749127/L.png)
[View diagram here](http://www.gliffy.com/go/publish/3749127/)
[Edit diagram here](http://www.gliffy.com/gliffy/#d=3749127&t=KeyHub_Schema)


#### Abbreviations

* str = nvarchar(256)
* str2 = nvarchar(4096)
* fk = foreign key
* pk = primary key
* n = nullable


### Notes

All date/time values are in UTC
All date/time columns will be in 'new' style (date, time, datetime2)

## Data Model 

### SKUs

* pk id
* fk private_key_id
* fk vendor_id
* str sku
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

* pk id
* fk vendor_id
* str code

### SKUFeatureIDs

* fk sku_id
* fk feature_id

### PrivateKeys

* pk id
* fk vendor_id
* str display_name
* binary(4096) private_key_bytes (encrypted by password in web.config)


### Vendors

* pk id
* nvarchar(1024) name
* street/city/region/postal/country

### Rights

* pk id
* nvarchar(1024) name

### UserObjectRights
* pk id
* fk user_id
* fk right_id
* fk object_id
* object_type


### Users

* pk id
* str name
* str email

* //TODO: OpenID integration fields

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

* pk guid id
* str display_name

### AppKeys

App keys can be created and deleted, but not edited. Simple. Their guid ID serves as their identifier. 

* pk guid id
* fk app_id


### AppLicenses

The interface must prevent an application from deleting its last license, as it would become orphaned.
Applications can have multiple licenses.

* fk license_id
* fk app_id

### DomainLicenses

Both automatic and manual licenses keys are stored here. 
For simplicity, we do not differentiate between domains and subdomains. In the .dll, we automatically strip "www." off domains, but other subdomains will require separate licenses.

* pk id
* fk license_id
* str domain_name
* datetime2 issued - The date the row was automatically or manually created
* n datetime2 expires - Manually created licenses may or may not have an expiration date. If there is no expiration, it should be null.
* bool is_automatic - Should be true if automatically generated
* binary(4096) license_bytes


### Packages

* pk id
* fk vendor_id
* str display_name

### PackageReleases

As an alternative to this table, Packages could simply reference an RSS XML feed... 

* pk id
* fk package_id
* str display_name - Will usually be something like "Resizer 4 alpha 1"
* str version - The official version, like "4.0.1"
* int stability - A stability rating, where 0 is stable, 1 is RC, 2 is beta, 3 is alpha, 4 is preview, etc.
* str2 notes_url - URL of release notes
* str2 download_url - URL of download. May need to be signed for public access if private on S3. 
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

* pk id
* str kind Ex. LicensingFailure, InsecureVersion, OutdatedVersion, CriticalErrors, Errors, Warnings

### UserAppNotifications

* pk id
* fk user_id
* fk app_id
* fk app_notification_kind_id

### TransactionItems

`In progress`

Questions - do we duplicate rows based on quantity, or tie many Licenses to a single transaction item?

* pk id
* fk sku_id
* int quantity
* e_item_name
* e_item_number


### Transactions

We may want to store all the original data in an XML column and only keep certain fields in the schema. 

`In progress`

Similar to PayPal IPN. See http://www.e-junkie.com/ej/help.integration.htm


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


### TransactionItem

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



