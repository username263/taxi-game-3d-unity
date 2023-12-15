import requests
from template_generator import generate_car

temp_group = generate_car('./Template(Dev).xlsx', 'Car')
res = requests.put('https://localhost:7170/Template/Car', json=temp_group, verify=False)

print(res.status_code)

