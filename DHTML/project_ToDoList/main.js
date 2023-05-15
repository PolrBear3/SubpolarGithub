let taskInput = document.getElementById("task-input");
let addButton = document.getElementById("add-button");
let allTasks = [];

addButton.addEventListener("click", Add_New_Task);

function Show_Task(){
    let resultHTML = '';
    
    for (let i = 0; i < allTasks.length; i++) {
        if (allTasks[i].isComplete == true){
            resultHTML += 
            `<div id="task-box" class="task-box">
                <div class="task-board">
                    <div class="task-complete">${allTasks[i].taskContent}</div>
                    <div class="task-buttons">
                        <button onclick="Task_Complete('${allTasks[i].id}')">Check</button>
                        <button onclick="Delete_Task('${allTasks[i].id}')">Delete</button>
                    </div>
                </div>
            </div>`
        }
        else{
            resultHTML += 
            `<div id="task-box" class="task-box">
                <div class="task-board">
                    <div>${allTasks[i].taskContent}</div>
                    <div class="task-buttons">
                        <button onclick="Task_Complete('${allTasks[i].id}')">Check</button>
                        <button onclick="Delete_Task('${allTasks[i].id}')">Delete</button>
                    </div>
                </div>
            </div>`
        }
    }

    document.getElementById("task-box").innerHTML = resultHTML;
}

function Add_New_Task(){
    let task = {
        id : Set_Task_ID(),
        taskContent : taskInput.value,
        isComplete : false
    };

    allTasks.push(task);
    Show_Task();
}
function Task_Complete(id){
    for (let i = 0; i < allTasks.length; i++) {
        if (id != allTasks[i].id) continue;
        allTasks[i].isComplete =! allTasks[i].isComplete;
        break;
    }
    Show_Task();
}
function Delete_Task(id){
    for (let i = 0; i < allTasks.length; i++){
        if (id != allTasks[i].id) continue;
        allTasks.splice(i, 1);
        break;
    }
    Show_Task();
}

function Set_Task_ID(){
    let trackID = 0;

    for (let i = 0; i < allTasks.length; i++) {
        if (allTasks[i].id != trackID) break;
        trackID++;
        continue;
    }

    return trackID;
}