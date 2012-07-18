# Data model

str = nvarchar(128)
f = foreign key
All date/time values are in UTC

### Users

* nvarchar(1024) OpenId

### UserOrganizationRoles

* f User
* f Org
* f Role

## Roles

* id
* str Name

(Roles 'Admin')

### Orgs/Vendors

* id
* str name

### PrivateKeys

* int id
* str name
* binary(4096) private key (password-encoded)
* f Org

### SKUs

* int id
* f Org - The organization
* f PrivateKey - The private key to use
* int MaxDelegatedUsers - The number of delegated users to permit
* int HoursBeforeLockingOwnershipData - The number of hours after purchase that ownership information can be editied by the end user. 
* int HoursAutoKeyLasts - The number of hours auto-generated license codes last before expiring.
* int MaxDomains - The maximum number of domains permitted.
* int NewDomainsPerSpan - The maximum number of new domains permitted in a given amount of time
* int NewDomainSpanHours - The number of hours 
* int MaxSubdomainsPerDomain - The 
* bool PermitManualKeyEdits
* datetime 


### SKUFeatures

* f SKU
* f Feature

### Features

Features are abilities unlocked by license codes. 

* int id
* str Name

### License

* id
* f SKU
* f Transaction
* guid secretkey



### DomainLicense



### Application

An application is a web.config instance, basically. You can have multiple web.config instances use the same application, but their errors will be pooled together.

* id 
* str DisplayName

### ApplicationKeys

These are not assymetric keys, they are keys that identify the application

* f Applicaiton
* str SecretKey

### UserApplicationSettings

* f User
* f Application
* b NotifyLicensingFailures
* b NotifyInsecureVersionInstalled
* b NotifyOutdatedVersionInstalled
* b NotifyUnresolvedErrors

### Transactions

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



