let taskInput = document.getElementById("task-input");
let addButton = document.getElementById("add-button");
let buttonBox = document.querySelectorAll(".button-box div")
let allTasks = [];
let filter = 'all';
let filterList = [];

addButton.addEventListener("click", Add_New_Task);

for (let i = 0; i < buttonBox.length; i++) {
    buttonBox[i].addEventListener("click", function(event){Change_Filter(event)});
}

function Show_Task(){
    let showTaskList = [];
    if (filter == 'all') showTaskList = allTasks;
    else if (filter == "not-done"|| filter == "done") showTaskList = filterList;

    let resultHTML = '';
    for (let i = 0; i < showTaskList.length; i++) {
        if (showTaskList[i].isComplete == true){
            resultHTML += 
            `<div id="task-box" class="task-box">
                <div class="task-board">
                    <div class="task-complete">${showTaskList[i].taskContent}</div>
                    <div class="task-buttons">
                        <button onclick="Task_Complete('${showTaskList[i].id}')">Check</button>
                        <button onclick="Delete_Task('${showTaskList[i].id}')">Delete</button>
                    </div>
                </div>
            </div>`
        }
        else{
            resultHTML += 
            `<div id="task-box" class="task-box">
                <div class="task-board">
                    <div>${showTaskList[i].taskContent}</div>
                    <div class="task-buttons">
                        <button onclick="Task_Complete('${showTaskList[i].id}')">Check</button>
                        <button onclick="Delete_Task('${showTaskList[i].id}')">Delete</button>
                    </div>
                </div>
            </div>`
        }
    }

    document.getElementById("task-box").innerHTML = resultHTML;
}

function Change_Filter(event){
    filter = event.target.id;
    filterList = [];

    if (filter == 'all') Show_Task();
    else if (filter == 'not-done'){
        for (let i = 0; i < allTasks.length; i++) {
            if (!allTasks[i].isComplete){
                filterList.push(allTasks[i]);
            }
        }
        Show_Task();
    }
    else if (filter == "done"){
        for (let i = 0; i < allTasks.length; i++) {
            if (allTasks[i].isComplete){
                filterList.push(allTasks[i]);
            }
        }
        Show_Task();
    }
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
        if (id == allTasks[i].id){
            allTasks.splice(i, 1);
            break;
        }
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