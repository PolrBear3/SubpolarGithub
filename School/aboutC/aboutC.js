const readline = require('readline').createInterface({
    input: process.stdin,
    output: process.stdout
  });

function Score_Check(){
    if (userNum >= 90) return "your grade is A";
    else if (userNum >= 80) return "your grade is B";
    else if (userNum >= 70) return "your grade is C";
    else if (userNum >= 60) return "your grade is D";
    else return "yout grade is F";
}

let userNum = 0;
readline.question("Enter your score: ", (userInput) => {
    userNum = parseInt(userInput);
    console.log(Score_Check());
    readline.close();
})