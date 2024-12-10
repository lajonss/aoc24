import sys

direction_symbols = "<^>v"
directions = [[0, -1], [-1, 0], [0, 1], [1,0]]



def find_guard(board):
  for y in range(len(board)):
    for x in range(len(board[0])):
      value = board[y][x]
      if value in direction_symbols:
        return Guard(y,x,value,board)

class Guard:
  def __init__(self,y,x,direction,board):
    self.y = y
    self.x = x
    self.direction = direction_symbols.index(direction)
    self.board=board
    
    
  def on_board(self):
    y=self.y
    x=self.x
    return y >= 0 and y < len(self.board) and x >= 0 and x < len(self.board[0])
    
  def advance(self):
    y=self.y
    x=self.x
    self.board[y][x] = "X"
    direction = directions[self.direction]
    y+=direction[0]
    x+=direction[1]
    try:
      if self.board[y][x] == "#":
        self.direction = (self.direction + 1) % len(directions)
        return
    except IndexError:
      pass
    self.y=y
    self.x=x
    
board = [[c for c in line.rstrip()] for line in sys.stdin]
guard = find_guard(board)
while guard.on_board():
  guard.advance()

result = sum([sum([1 for char in line if char == "X"]) for line in board])
print(result)
