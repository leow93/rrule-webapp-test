import http from 'k6/http'

export default function () {
  http.get('http://localhost:3000/instances/2023-05-01/2023-05-31')
}
