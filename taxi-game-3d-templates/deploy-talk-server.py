import requests
from template_generator import generate_talk

temp_group = generate_talk('./Template(Dev).xlsx', 'Talk')
res = requests.put('https://localhost:7170/Template/Talk', json=temp_group, verify=False)

print(res.status_code)