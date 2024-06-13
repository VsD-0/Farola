function printProfessionalInfo(surname, name, patronymic, profession, specialization, information) {
const printContent = `
    <!DOCTYPE html>
    <html lang="ru">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Farola - Специалист: ${surname} ${name}</title>
      <style>
        body {
          font-family: Arial, sans-serif;
          font-size: 14px;
          margin: 20px;
        }
        h1 {
          font-size: 24px;
          font-weight: bold;
          color: #333;
        }
        .professional-info {
          border: 1px solid #ddd;
          padding: 15px;
          margin-bottom: 20px;
        }
        .professional-info label {
          display: block;
          margin-bottom: 5px;
          font-weight: bold;
        }
        .professional-info p {
          margin-bottom: 0;
        }
      </style>
    </head>
    <body>
      <h1>Farola - Специалист: ${surname} ${name}</h1>
      <div class="professional-info">
        <label>ФИО:</label>
        <p>${surname} ${name} ${patronymic ? patronymic : ''}</p>
      </div>
      <div class="professional-info">
        <label>Профессия:</label>
        <p>${profession}</p>
      </div>
      <div class="professional-info">
        <label>Специализация:</label>
        <p>${specialization}</p>
      </div>
      <div class="professional-info">
        <label>Информация:</label>
        <p>${information}</p>
      </div>
    </body>
    </html>
  `;
  const printWindow = window.open("", "_blank", "width=800,height=600");
  printWindow.document.write(printContent);
  printWindow.document.close();
  printWindow.focus();
  printWindow.print();
  printWindow.close();
}