import json
from openpyxl import load_workbook

book = load_workbook('./Template(Dev).xlsx', data_only=True)

sheet = book['Stage']

skip = 0
temp_group = []
for row in sheet.rows:
    # 두번째 줄부터 데이터 저장
    if skip < 1:
        skip += 1
        continue
    
    # ID컬럼이 비어 있으면 추가하지 않음
    if row[0].value == None:
        continue

    new_temp = {
        'Id': row[0].value,
        'Scene': row[1].value
    }
    temp_group.append(new_temp)

client_path = '../taxi-game-3d-client/Assets/_TaxiGame/Resources/Templates/Stage.json'
with open(client_path, 'w', encoding='UTF-8-sig') as f:
    json.dump(temp_group, f, ensure_ascii=False)
