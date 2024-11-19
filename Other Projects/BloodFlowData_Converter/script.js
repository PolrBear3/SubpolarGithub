document.getElementById('csvFileInput').addEventListener('change', handleFileSelect);

function handleFileSelect(event)
{
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e)
    {
        const text = e.target.result;
        const data = parseCSV(text);

        createChart(data.dates, data.bpmData, data.pefvData, data.spo2Data);
    };
    reader.readAsText(file);
}

function parseCSV(data)
{
    const rows = data.split('\n').slice(1); // Skip header row
    const dates = [];
    const bpmData = [];
    const pefvData = [];
    const spo2Data = [];

    rows.forEach(row =>
    {
        const [id, name, birthday, mobile, email, wrist_length, bpm, pefv, spo2, created_at] = row.split(',');
        if (created_at)
        {
            dates.push(created_at.split(' ')[0]);
            bpmData.push(parseFloat(bpm));
            pefvData.push(parseFloat(pefv));
            spo2Data.push(parseFloat(spo2));
        }
    });

    return { dates, bpmData, pefvData, spo2Data };
}

function createChart(dates, bpmData, pefvData, spo2Data)
{
    const ctx = document.getElementById('lineChart').getContext('2d');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: dates,
            datasets: [
                {
                    label: 'BPM',
                    data: bpmData,
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    fill: false,
                    tension: 0.4
                },
                {
                    label: 'PEFV',
                    data: pefvData,
                    borderColor: 'rgba(54, 162, 235, 1)',
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    fill: false,
                    tension: 0.4
                },
                {
                    label: 'SpO2',
                    data: spo2Data,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    fill: false,
                    tension: 0.4
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Dates'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Values'
                    }
                }
            }
        }
    });
}