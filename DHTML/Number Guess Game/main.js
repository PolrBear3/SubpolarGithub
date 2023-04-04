let randNum = 0;
let chances = 5;
let gameOver = false;
let history = [];

let playButton = document.getElementById("Play_Button");
let resetButton = document.getElementById("Reset_Button");
let userInput = document.getElementById("User_Input");
let resultArea = document.getElementById("Result_Area");
let chanceArea = document.getElementById("Chance_Area");

playButton.addEventListener("click", Play); //click, focus, hover
resetButton.addEventListener("click", Reset);
userInput.addEventListener("focus", function(){userInput.value = null})

function Get_Random_Number(){
  randNum = Math.floor(Math.random()*100)+1;
  console.log("Answer: " + randNum);
}

Get_Random_Number();

function Play(){
  let userValue = userInput.value;

  if (userValue < 1 || userValue > 100){
    resultArea.textContent = "1에서 100사이의 숫자를 입력해주세요!";
    return;
  }
  if (history.includes(userValue)){
    resultArea.textContent="이미 입력한 값입니다!";
    return;
  }

  chances--;
  chanceArea.innerHTML=`남은기회: ` + chances + `번`;
  history.push(userValue);

  if (userValue < randNum){
    resultArea.textContent = "Up"
  }
  else if (userValue > randNum){
    resultArea.textContent = "Down"
  }
  else{
    resultArea.textContent = "Correct"
    gameOver = true;
    playButton.disabled = true;
  }

  GameOver_Check();
}

function Reset(){
  Get_Random_Number();
  userInput.value = null;
  resultArea.textContent = "결과가 나온다"
  chances = 5;
  gameOver = false;
  history=[];
  chanceArea.innerHTML=`남은기회: ` + chances + `번`;
  playButton.disabled = false;
}

function GameOver_Check(){
  if (chances > 0) return;
  gameOver = true;

  if (gameOver){
    playButton.disabled = true;
    resultArea.textContent = "Game Over"
  }
}