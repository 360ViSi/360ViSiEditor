# Simulation JSON structure
## Class VideoFile
- **string** videoFileName
  - "None" means no video only actions
- **int** videoID
  - positive integers (-1 is reserved for end)
- **Action[]** actions

## Class Action
- **string** actionText
  - "auto" means if video get to end then this action
- **int** nextVideo
  - -1 end of simulation

## File structure
- List of video objects:
