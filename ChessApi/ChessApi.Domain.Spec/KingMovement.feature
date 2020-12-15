Feature: King Movement
    In order to make a valid move
    As a chess player
    I want to know the rules for the specific movement of a king

@mytag
Scenario: A king may move one step in any direction
    Given an empty board 
    And a King that starts on 'f5'
	When It moves
    Then It can move to all 'V's and cannot move to all '.'s
|   | a | b | c | d | e | f | g | h |
| 8 | . | . | . | . | . | . | . | . |
| 7 | . | . | . | . | . | . | . | . |
| 6 | . | . | . | . | V | V | V | . |
| 5 | . | . | . | . | V | K | V | . |
| 4 | . | . | . | . | V | V | V | . |
| 3 | . | . | . | . | . | . | . | . |
| 2 | . | . | . | . | . | . | . | . |
| 1 | . | . | . | . | . | . | . | . |

