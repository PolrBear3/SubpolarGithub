var currentPoints = 0;

function AddUp(){
    for (let index = 0; index < 10; index++) {
        currentPoints += 1;
    }
}

function Print_CurrentPoint(){
    console.log(currentPoints);
}

AddUp();
AddUp();
Print_CurrentPoint();

