Feature: ItemExpiredNotification
	In order to keep track of expired items
	As a system client
	I want to be notified when an item has expired

Scenario: Notification that an item has expired
	Given an item with low expiration date:
	| Label    | ItemType | ExpirationDate |
	| Label1   | TypeA    | 12/09/2017     |
	And the item has been added to the inventory previously
	When an item expires
	Then there is a notification about the expired item