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
* str8 = nvarchar(8)
* str512 = nvarchar(512)
* str1k = nvarchar(1024)
* str4k = nvarchar(4096)
* fk = foreign key
* pk = primary key)
* n = nullable


### Notes

All date/time values are in UTC
All date/time columns will be in 'new' style (date, time, datetime2)
All date values will be presented in day, full month name, year format. Example: 18 september 2012 

## Data Model 

### SKUs

* pk id
* fk private_key_id
* fk vendor_id
* str256 sku - The short SKU code, like 'R3Bundle1Ent'
* str512 name - The display name for the SKU, like 'Resizer 3 Enterprise Performance Bundle'
* n str256 alt_sku - An alternate code used as a fallback for matching up transaction cart items based on 'item_number', like '929356||R3Bundle1Ent'
* n int max_domains - The maximum number of domain licenses permitted by this license
* n int edit_ownership_duration - How long the Owner_* fields are editable after license is issued
* n int max_support_contacts - Maxmimum number of users listed as a support contact
* n int change_support_contacts_duration - How long the assigned support contact can be changed
* n int license_duration - How long the license is valid for
* n int auto_domain_duration - How long auto-generated domain licenses are valid before they must be auto-renewed. 
                               Removing expired domain licenses (to make room for new ones) will be part of the license validation process. 
							   Domainlicenses will be removed from db (no logical delete).
* n int manual_domain_duration - How long manually generated domain licenses are valid for
* bool can_delete_manual_domains - If true, users who have the EditDomains right on the license can delete manual licenses to make room for more or do cleanup. 
* bool can_delete_auto_domains - If true, users can delete auto-generated licenses to make room for more or do cleanup
* n datetime2 release_date - When this SKY is first offered for purchase
* n datetime2 expiration_date - When this SKU is no longer available for purchase
								Integrate this check into the webshop REST service. Provides error message back if SKU has expired. SKU will not be part of the transaction.

(All duration fields are in number of days)

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

### Entities

* Name
* Department
* Street
* City
* Region
* Postal
* Country
* Billing_Email - 
* bool Billing_Email_Confirmed

### Licenses

* pk id
* fk sku_id
* fk transaction\_item_id
* str owner\_name - Name of first owner. Filled out by user during 'claiming' process. Not editable after SKU.edit\_ownership_duration expires.
* fk purchasing\_entity_id - Always editable.
* fk owning\_entity\_id - Always editable. Vendor admins can view mismatches between owner\_name and owning\_entity_id, and we can even display a warning to the user that licenses are not transferable...
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

Question: TransactionItems and Transaction contain a number of fields required for handling the payment.
          Webshop is responsible of payment handling. As soon as KeyHub recieves a transaction request, the user can start claiming the license.
          Suggestion to remove all payment related fields such as TransactionItem.Gross.

Answer: Many hosted webshops do NOT provied an API for transaction querying. This is the case with e-junkie and many of the services I have seen. Thus, any reporting, transaction lookup, or discount coupon calculation will have to be done against the keyhub copy of transaction data. Thus, it is critical (for Imazen, at least), that all transaction details be stored here. 

Of course, this can be a vendor-specific choice, and all fields can be marked nullable. It shouldn't be hard to optionally 'drop' fields based on the vendor.

Note: 'mc_' is for multi-currency fields. If we don't need that clarification, we can drop the prefix.

### TransactionItems table

* pk id (guid) - Autogenerated identity field
* n fk sku_id - SKU row id (nullable, because it's possible that we won't be able to match it up to an SKU)
* n str256 sku - The textual SKU value extracted from item_number OR recieved via a per-product POST. The text value used to lookup sku_id, when combined with the vendor value contained in the POST URL
* fk transaction_id - Parent transaction id (guid)
* int cart_positon - 1-based number indicating the position of this item in the cart. Non-unique, meaning an item with qty=2 will generate two Transactionitems with cart_position=1

The following are PayPal IPN fields. If SKU lookup fails, they are all displayed. Otherwise, just mc_gross is used in combination with the linked SKU info.

* n float mc_gross - (Gross sale price of transaction item. Will need to be divided by quantity during import) 
* str1k item_name - The generic display name shown during checkout, like "Resizer 3"
* str1k item_number - A pass-through textual variable, like "929356||R3Bundle1Ent", usually limited to 127 chars by paypal, but can be long with other providers. E-junkie combines the internal item number with the SKU.
* n str512 option_name1 - (if applicable) if you are using any options with your products, then this will contain first option's name
* n str512 option_selection1 - (if applicable) if you are using any options with your products, then this will contain first option's value that buyer selected
* n str512 option_name2
* n str512 option_selection2
* n str512 option_name3
* n str512 option_selection3

* n str1k vendor_comments - Permits the vendor to make comments about a line item purchase, such as to record that the user has already exhausted their support package, etc.

Note option1 and option2 above are used for the 'license type' and 'bundle' values for Resizer, and for 'type' and 'duration' of support contracts.

KeyHub MUST be able to recieve and store transactions and transaction items that don't map to existing SKU objects. E-junkie doesn't offer any API for accessing transaction data, except for csv export. It's important for me to be able to look up support purchases along with license information.


### Transactions table


The following fields are all PayPal IPN standard, and are shared by most payment processors. 

See [PayPal documentation for more details](https://cms.paypal.com/us/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_html_IPNandPDTVariables) about PayPal's use of these fields.

See [E-junkue documentation](http://www.e-junkie.com/ej/help.integration.htm) for details on how it maps other payment provider data into the same fields.


* pk id (guid) - Autogenerated identity field
* fk vendor_id - The vendor associated with this transaction (filled by the URL IPN path given to the payment processor/webshop)
* n fk purchasing_entity_id - The purchasing entity for the transaction (can be auto-filled if the entity has a confirmed e-mail address matching 'payer_email')
* str1k transaction - (txn_id) Transaction ID generated by payment processor (for non-PayPal txns we add a prefix: gc- for google checkout, au- for authorize.net, 2co- for 2checkout, cb- for clickbank and tp- for trialpay payments).
* n str32 transaction_type - (txn_type) Paypal transaction type or alternate payment provider code, such as 'cart', 'web_accept','expresscheckout', 'ppdirect', 'gc_cart','authnet','cb', '2co_cart','tp','ejgift', etc
* n str1k invoice - Pass-through invoice ID
* datetime2 payment_date - Either the time payment was initiated or completed.
* n str256 custom - A custom pass-through field we can use to simplify purchasing for logged-in users.
* n str32 charset - this should normally be 'utf-8', as e-junkie converts order data into UTF-8 from whatever charset was sent by the payment processor (in very rare cases this may identify the original charset if conversion to UTF-8 cannot be done)


* n str8 mc_currency - The currency used for all the monetary values in the transaction
* n float mc_gross - Full amount of the customer's payment, before transaction fee is subtracted. 
* n float mc_shipping - Portion of mc_gross allocated to shipping costs
* n float mc_handling -  Portion of mc_gross allocated to handling costs
* n float tax - Tax applied to the order
* n float mc_fee - Transaction fee associated with the payment. mc_gross minus mc_fee equals the amount deposited into the receiver_email account. 
* str32 payment_status - 'Completed', 'Pending', etc.
* n str32 pending_reason
* n str32 payment_type - 'instant', 'echeck'


* n bool payer_verified - PayPal Verified Buyer
* n str1k payer_email - May be the paypal billing e-mail, google checkout email, or a masked google checkout e-mail
* n str64 payer_id - Unique paypal customer ID
* n str1k first_name - Billing first name
* n str1k last_name - Billing last nae
* n str1k payer_business_name - billing address company name (if provided)
* n str256 payer_phone - billing phone
* n str256 payer_street - for two-line addresses, this variable passes both lines, separated by a newline character
* n str64 payer_city
* n str64 payer_state
* n str20 payer_zip
* n str64 payer_country
* n str8 payer_country_code - 2-letter ISO country code

* n str8 residence_country - ISO 3166 country code billing country (if supported by payment processor)



* n bool address_confirmed - Paypal Verified Address
* n str256 address_name - ship-to full name if different from billing (buyer) name
* n str256 address_business_name - shipping address company name (if provided)
* n str256 address_street - for two-line addresses, this variable passes both lines, separated by a newline character
* n str64 address_city
* n str64 address_state
* n str20 address_zip
* n str64 address_country
* n str8 address_country_code - shipping address 2-letter ISO country code
* n str20 address_phone

* n int num_cart_items - The number of transaction items (Prior to expansion based on qty field)



* n str256 vendor_email - (business) The e-mail or PayPal ID of the vendor. (uses 'business' field or 'from_email')



*Non-PayPal standard fields:*

* n str256 vendor_name - The name of the vendor (uses 'from_name')
* n str1k discount_codes - A list of discount codes used by the customer. 
* n str256 buyer_ip - The IP address of the purchaser.
* n xml gc_xml - XML from Google Checkout
* n xml other_data - XML containing all 'other' fields passed in. Should be small, and typically contains:

	mailing_list_status=true
	client_id=41912
	client_shipping_method_id=0
	ej_txn_id=14521369


### Per-Order vs. Per-product POSTs

E-junkie supports per-order and per-product POSTs. Order-level POSTs contain everything product-level posts contain, except:

* item_cart_position - this identifies which particular item in the order triggered a given submission to a product-specific Payment Variable Information URL (which receives the full data for all items in the order), so you can match this to the value of X in the Item Specific IPN Data variable names (listed above) to identify which details in the submission pertain to that particular item -- e.g., if you receive item_cart_position=2, you'll know that submission was triggered by the second item in the buyer's cart, corresponding to item_name2, item_number2, etc.
* sku - only sent to the Payment Variable Info URL for items configured with Variants having individual price/weight/stock/SKU, this is the SKU you'd configured for the particular Variant of that item which the buyer ordered.
* expiry_hours - (if applicable) maximum number of Hours you allow your download links to be valid for in your product configuration
* max_downloads - (if applicable) maximum number of download Attempts you allow in your product configuration
* affiliate_fee - (if applicable) commission earned on the item by your E-junkie affiliate
* key - (if applicable) stored or generated code (key/license/serial/PIN) sent to the buyer (this variable is not sent to keygen URLs)

Thus, the recieving code should ether ignore duplicate POSTs with the same txn_id value ('transaction' in the db), OR update the transaction's 'payment_status' column.


### Validating posts from e-junkie

Incoming transaction posts (for e-junkie) are validated through the 'handshake' querystring.


handshake - md5(your_e-junkie_login_email+md5(your-e-junkie-password))
You can use this variable to ensure that the data is coming from E-junkie's server. We take an md5 hash of your E-junkie password, then tack your E-junkie login email in front of that hash, then re-hash the whole thing in md5 again. If your E-junkie login email and password became known to someone else, they could forge this hash. If you ever change your E-junkie login email and/or password, you'd need to update the hash reference in your keygen script. You can use any scripting language of your choice to compare the handshake hash we pass against a matching hash at your end. Here's an example using PHP:

	<?php
	// Put this in the top of your script, so if the handshake
	// does not match, it will exit, but otherwise it runs the
	// rest of your script:
	  if ($_POST['handshake']!==md5("your@login.email".md5("your_e-junkie_password")))
	  {
	    exit;
	  }else {
	// Here is where you do whatever you wish with the order
	// data variables. To illustrate the point, this example
	// will just email the data to a given address:
	    mail ("your@email.address.com","post from e-junkie", print_r($_POST,true));
	  }
	?>

### Example product-specific transaction post (2 posts, 1 per transaction item)



Similar to PayPal IPN. See http://www.e-junkie.com/ej/help.integration.htm



payment_date=06%3A58%3A04+Sep+15%2C+2012+MST
payer_email=paypal%40sedonalace.com
address_name=
address_state=
address_country=
address_city=
address_zip=
address_street=
address_country_code=
first_name=Jason
last_name=Rockenbach
payer_id=Q6T6WMNUD7N3A
residence_country=US
payer_status=verified
invoice=wc8dwxjugr6dja07lyv2kax444488wsgssosgg
address_status=
payer_business_name=Sedona+Lace%2C+LLC
payer_phone=
custom=
mc_currency=USD
business=billing%40imazen.io
mc_gross=198
mc_shipping=0.00
tax=0.00
item_name1=Resizer+3
item_number1=929356%7C%7CR3Bundle1Pro
mc_gross_1=99
quantity1=1
item_name2=Resizer+3
item_number2=929356%7C%7CR3Bundle3Pro
mc_gross_2=99
quantity2=1
num_cart_items=2
pending_reason=None
txn_id=5MK79527CF849221P
payment_status=Completed
txn_type=expresscheckout
payment_type=instant
mc_fee=5.25
mailing_list_status=true
client_id=41912
charset=utf-8
charset_assumed=true
buyer_ip=72.186.165.51
handshake=ff35a320762dcec799d9c0bb9831577c
discount_codes=
from_name=Imazen
from_email=billing%40imazen.io
mailing_list_status=true
client_shipping_method_id=0
item_cart_position=1
item_number=929356
sku=R3Bundle1Pro
expiry_hours=0
max_downloads=9
ej_txn_id=15244444



payment_date=06%3A58%3A04+Sep+15%2C+2012+MST
payer_email=paypal%40sedonalace.com
address_name=
address_state=
address_country=
address_city=
address_zip=
address_street=
address_country_code=
first_name=Jason
last_name=Rockenbach
payer_id=Q6T6WMNUD7N3A
residence_country=US
payer_status=verified
invoice=wc8dwxjugr6dja07lyv2kax444488wsgssosgg
address_status=
payer_business_name=Sedona+Lace%2C+LLC
payer_phone=
custom=
mc_currency=USD
business=billing%40imazen.io
mc_gross=198
mc_shipping=0.00
tax=0.00
item_name1=Resizer+3
item_number1=929356%7C%7CR3Bundle1Pro
mc_gross_1=99
quantity1=1
item_name2=Resizer+3
item_number2=929356%7C%7CR3Bundle3Pro
mc_gross_2=99
quantity2=1
num_cart_items=2
pending_reason=None
txn_id=5MK79527CF849221P
payment_status=Completed
txn_type=expresscheckout
payment_type=instant
mc_fee=5.25
mailing_list_status=true
client_id=41912
charset=utf-8
charset_assumed=true
buyer_ip=72.186.165.51
handshake=ff35a320762dcec799d9c0bb9831577c
discount_codes=
from_name=Imazen
from_email=billing%40imazen.io
mailing_list_status=true
client_shipping_method_id=0
item_cart_position=2
item_number=929356
sku=R3Bundle3Pro
expiry_hours=0
max_downloads=9
ej_txn_id=15244444


### Parsing individual transaction items

Here, X represents the position of each item in the buyer's cart:

* item_nameX
* item_numberX - item number you have set in product configuration
* quantityX - quantity sold
* mc_gross_X - sale price for this product * quantity sold
* option_name1_X - (if applicable) if you are using any options with your products, then this will contain first option's name
* option_selection1_X - (if applicable) if you are using any options with your products, then this will contain first option's value that buyer selected
* option_name2_X
* option_selection2_X
* option_name3_X
* option_selection3_X



### Exported Transactions E-junkie Schema (1 row per transaction item):


	Payment Date (MST)	Processed by E-j (MST)	Transaction ID	Payment Processor	E-j Internal Txn ID	Payment Status	First Name	Last Name	Payer E-mail	Billing Info	Payer Phone	Card Last 4	Card Type	Payer IP	Passed Custom Param.	Discount Codes	Invoice	Shipping Info	Shipping Phone	Shipping	Tax	eBay Auction Buyer ID	Affiliate E-mail	Affiliate Name	Affiliate ID		Currency	Item Name	Variations/Variants	Item Number	SKU	Quantity	Amount	Affiliate Share (per item)	Download Info	Key/Code (if any)	eBay Auction ID	Buyer Country
	6/8/11 2:24	6/8/11 2:24	gc-363976288355594	Google Checkout	9476313	Completed	Andrew	R Ward	Andrew-bjk6pjj1dmn@checkout.google.com	45 Whitfield St, London, LONDON, W1T4HD, United Kingdom (Great Britain)				83.244.237.98		For Cart Item Total: 60OFFLOYALTY, 	3.63976E+14	Andrew R Ward, 45 Whitfield St, London, LONDON, W1T4HD, United Kingdom (Great Britain)		0	0					0	USD	Resizer 3	Bundle:Performance Bundle, License:Pro License	929356	R3Bundle1Pro	1	39.6	0	1 attempt(s), Last by 83.244.237.98 @ 2011-06-08 02:24:23			United Kingdom (Great Britain)

