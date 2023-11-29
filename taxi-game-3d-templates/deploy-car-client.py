import json
from openpyxl import load_workbook

book = load_workbook('./Template(Dev).xlsx', data_only=True)

sheet = book['Car']

skip = 0
temp_group = []
for row in sheet.rows:
    # 세번째 줄부터 데이터 저장
    if skip < 2:
        skip += 1
        continue
    
    # ID컬럼이 비어 있으면 추가하지 않음
    if row[0].value == None:
        continue

    new_temp = {
        'Id': row[0].value,
        'Name': {
            'Table': row[1].value,
            'Key': row[2].value
        },
        'Icon': row[3].value,
        'Prefab': row[4].value,
        'Cost': row[5].value
    }
    temp_group.append(new_temp)

client_path = '../taxi-game-3d-client/Assets/_TaxiGame/Resources/Templates/Car.json'
with open(client_path, 'w', encoding='UTF-8-sig') as f:
    json.dump(temp_group, f, ensure_ascii=False)
