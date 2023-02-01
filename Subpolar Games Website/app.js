function Calculate() {
  var num = parseInt(document.getElementById("actualNum").value);
  var multiplyNum = parseInt(document.getElementById("timesNum").value);
  var sum = 0;

  for (var i = 0; i < multiplyNum; i++) {
    sum += num;
  }

  alert(sum);
}