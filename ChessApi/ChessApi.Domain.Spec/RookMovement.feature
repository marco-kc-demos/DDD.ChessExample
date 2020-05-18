Feature: RookMovement
    In order to kwow the chess rules
    As a chess player
    I want to know the rules for the specific movement of a rook

@mytag
Scenario: A rook may only move horizontally and vertically
    Given an empty board 
    And a Rook that starts on 'd5'
	When It moves
    Then It can move to all 'V's and cannot move to all '.'s
|   | a | b | c | d | e | f | g | h |
| 8 | . | . | . | V | . | . | . | . |
| 7 | . | . | . | V | . | . | . | . |
| 6 | . | . | . | V | . | . | . | . |
| 5 | V | V | V | R | V | V | V | V |
| 4 | . | . | . | V | . | . | . | . |
| 3 | . | . | . | V | . | . | . | . |
| 2 | . | . | . | V | . | . | . | . |
| 1 | . | . | . | V | . | . | . | . |

