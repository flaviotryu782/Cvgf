import { createClient } from 'https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2/+esm';

const SUPABASE_URL = 'https://owwdkyayohwrnagramcp.supabase.co';
const SUPABASE_ANON_KEY = 'sb_publishable_i1sYpTp67FYqJtpP83rWyQ_pQsTd7fX';
const TABLE_NAME = 'news_posts';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

const newsGrid = document.getElementById('newsGrid');
const latestNewsList = document.getElementById('latestNewsList');
const publishForm = document.getElementById('publishForm');
const publishStatus = document.getElementById('publishStatus');

function formatTime(dateString) {
  const d = new Date(dateString);
  if (Number.isNaN(d.getTime())) return '--:--';
  return d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}

function imageByCategory(category) {
  const normalized = category.toLowerCase();
  if (normalized.includes('saúde')) return 'images/vacinacao.svg';
  if (normalized.includes('econom')) return 'images/credito-verde.svg';
  return 'images/infraestrutura-digital.svg';
}

function setStatus(message, isError = false) {
  if (!publishStatus) return;
  publishStatus.textContent = message;
  publishStatus.classList.toggle('error', isError);
}

function removeDynamicNodes() {
  newsGrid?.querySelectorAll('[data-dynamic="true"]').forEach((card) => card.remove());
  latestNewsList?.querySelectorAll('[data-dynamic="true"]').forEach((item) => item.remove());
}

function createTextElement(tag, text, className) {
  const el = document.createElement(tag);
  if (className) el.className = className;
  el.textContent = text;
  return el;
}

function renderNews(posts) {
  removeDynamicNodes();
  if (!newsGrid || !latestNewsList) return;

  posts.forEach((post) => {
    const article = document.createElement('article');
    article.className = 'article-card';
    article.dataset.dynamic = 'true';

    const image = document.createElement('img');
    image.src = imageByCategory(post.category);
    image.alt = `Ilustração da categoria ${post.category}`;
    image.loading = 'lazy';

    const meta = createTextElement('p', `${post.category} • ${post.author} • ${formatTime(post.created_at)}`, 'article-meta');
    const title = createTextElement('h3', post.title);
    const summary = createTextElement('p', post.summary);

    article.append(image, meta, title, summary);
    newsGrid.prepend(article);

    const latest = document.createElement('li');
    latest.dataset.dynamic = 'true';
    const time = createTextElement('time', formatTime(post.created_at));
    latest.append(time, document.createTextNode(post.title));
    latestNewsList.prepend(latest);
  });
}

async function loadPosts() {
  const { data, error } = await supabase
    .from(TABLE_NAME)
    .select('id,title,category,summary,author,created_at')
    .order('created_at', { ascending: false })
    .limit(6);

  if (error) {
    setStatus('Não foi possível carregar notícias do Supabase. Confira tabela/RLS.', true);
    return;
  }

  renderNews(data ?? []);
}

if (publishForm) {
  publishForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const formData = new FormData(publishForm);
    const payload = {
      title: String(formData.get('title') ?? '').trim(),
      category: String(formData.get('category') ?? '').trim(),
      author: String(formData.get('author') ?? '').trim(),
      summary: String(formData.get('summary') ?? '').trim(),
    };

    const hasEmpty = Object.values(payload).some((value) => !value);
    if (hasEmpty) {
      setStatus('Preencha todos os campos.', true);
      return;
    }

    const { error } = await supabase.from(TABLE_NAME).insert(payload);

    if (error) {
      setStatus('Falha ao publicar no Supabase. Valide políticas RLS e estrutura da tabela.', true);
      return;
    }

    setStatus('Notícia publicada com sucesso no Supabase.');
    publishForm.reset();
    await loadPosts();
  });
}

loadPosts();
