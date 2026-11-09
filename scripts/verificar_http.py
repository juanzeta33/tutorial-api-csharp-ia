#!/usr/bin/env python3
"""Ejecutar contra una BD de laboratorio aislada; crea y elimina sus propios datos."""
import json,sys,urllib.request,urllib.error
from pathlib import Path
base=sys.argv[1] if len(sys.argv)>1 else 'http://localhost:5210'
evidence=[];created=[]
def request(method,path,body=None):
 payload=None if body is None else json.dumps(body).encode()
 req=urllib.request.Request(base+path,data=payload,method=method,headers={'Content-Type':'application/json'})
 try:r=urllib.request.urlopen(req,timeout=10)
 except urllib.error.HTTPError as e:r=e
 text=r.read().decode();row={'method':method,'path':path,'status':r.status,'body':json.loads(text) if text else None,'location':r.headers.get('Location'),'empty':not text};evidence.append(row);return row
try:
 r=request('GET','/api/productos');assert r['status']==200
 r=request('POST','/api/productos',{'nombre':'  Cuaderno  ','precio':4.50,'existencias':8});assert r['status']==201;r_id=r['body']['id'];created.append(r_id);assert r['location'].endswith('/api/productos/'+str(r_id));assert r['body']['nombre']=='Cuaderno'
 r=request('GET','/api/productos/'+str(r_id));assert r['body']['precio']==4.5
 for body in [dict(nombre='   ',precio=1,existencias=1),dict(nombre='X',precio=-1,existencias=1),dict(nombre='X',precio=1,existencias=-1),dict(nombre='X',precio=1.234,existencias=1),dict(nombre='X',existencias=1)]:
  assert request('POST','/api/productos',body)['status']==400
 r=request('PUT','/api/productos/'+str(r_id),{'nombre':'Cuaderno grande','precio':6.25,'existencias':4});assert r['status']==204 and r['empty']
 r=request('GET','/api/productos/'+str(r_id));assert r['body']['nombre']=='Cuaderno grande' and r['body']['precio']==6.25 and r['body']['existencias']==4
 assert request('PUT','/api/productos/'+str(r_id),{'nombre':'Parcial','existencias':1})['status']==400
 assert request('GET','/api/productos/2147483647')['status']==404
 assert request('PUT','/api/productos/2147483647',{'nombre':'X','precio':1,'existencias':1})['status']==404
 assert request('GET','/api/productos?page=0')['status']==400
 assert request('GET','/api/productos?pageSize=101')['status']==400
 r=request('DELETE','/api/productos/'+str(r_id));assert r['status']==204 and r['empty'];created.remove(r_id)
 assert request('GET','/api/productos/'+str(r_id))['status']==404
 assert request('DELETE','/api/productos/'+str(r_id))['status']==404
 # Swagger y su contrato se comprueban además del CRUD.
 assert urllib.request.urlopen(base+'/swagger/index.html',timeout=10).status==200
 spec=json.load(urllib.request.urlopen(base+'/openapi/v1.json',timeout=10));assert '/api/productos' in spec['paths'];assert {'get','post'}<=set(spec['paths']['/api/productos'])
 Path('evidencia/http.json').write_text(json.dumps(evidence,ensure_ascii=False,indent=2));print(f'{len(evidence)} peticiones verificadas + Swagger UI y documento OpenAPI.')
finally:
 for item in created:request('DELETE','/api/productos/'+str(item))
