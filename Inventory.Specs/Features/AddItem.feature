Feature: Add an item to the inventory
	In order to query the inventory for future reference
	As a user of the API
	I want to add an item to the inventory

Scenario: Add an item to the inventory
	Given an item with the following fields:
	| Label    | ItemType | ExpirationDate |
	| Label1   | TypeA    | 31/12/2020     |
	When I add the new item to the inventory
	Then the inventory contains information about the newly added item
